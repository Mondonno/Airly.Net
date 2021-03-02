using System;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

using System.Net;
using System.Collections.Concurrent;
using System.Linq;

namespace AirlyAPI.Handling
{
    // == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==
    //   TODO: Przekształcić zwracaną wartość z RequestModule na RawResponse i parsowanie zrobić z poziomu queuera (parse method)
    // == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==

    // Simple wrapper for semaphore slim to handle threads in requests
    public class Waiter : SemaphoreSlim
    {
        public Waiter() : base(1){} // Limiting to one action per push and one thread (so user can make a lot of threads and the thread does not have childs)

        public int RemainingTasks() => this.CurrentCount; // Shortcut
        public void Destroy() => this.Dispose();
    }

    // The queuer make the same what do waiter but waiter is a core for queuer
    public class RequestQueuer
    {
        public Waiter Waiter { get; private set; }
        public RESTManager Manager { get; private set; }

        public bool RateLimited { get; private set; } = false; // The ratelimit error can be handled by user so thats why this variable exists
        public bool Inactive { get => Waiter.RemainingTasks() == 0 && !this.RateLimited; }

        public RequestQueuer(RESTManager manager)
        {
            this.Manager = manager;
            this.Waiter = new Waiter();
        }

        public async Task<JToken> Push(RequestModule request)
        {
            await Waiter.WaitAsync();

            // Simple thread locks handlings
            try { return await this.Make(request); }
            finally { Waiter.Release(); }
        }

        public void Zero()
        {
            this.Waiter.Destroy(); // Deleting all the pending requests
            this.RateLimited = false;
        }

        // simple converter wrap
        private JToken ConvertJsonString(string json)
        {
            JToken token = JsonParser.ParseJson(json);
            return token;
        }
        private JToken ConvertJsonString(AirlyResponse res) => ConvertJsonString(res.rawJSON);

        private async Task<JToken> Make(RequestModule request)
        {
            Handler handler = new Handler();
            AirlyResponse res;

            try { res = await request.MakeRequest(); }
            catch (Exception ex) { throw new AirlyError(new HttpError(ex)); }

            if (res == null || string.IsNullOrEmpty(res.rawJSON)) throw new HttpError("Can not resolve the Airly api response");
            if (this.RateLimited)
            {
                var details = handler.GetRateLimitDetails(res.response);
                if (details.IsRateLimited) handler.MakeRateLimitError(res, new Utils(), null);
                else {
                    this.RateLimited = false;
                    System.Diagnostics.Debug.WriteLine("The ratelimited is detected but it is not authenticated by headers");
                }
            }

            ErrorModel parsedError = handler.GetErrorFromJSON((JObject)res.JSON);

            int statusCode = (int) res.response.StatusCode;

            if (statusCode == 200) return ConvertJsonString(res); // Nothing wrong with request & response
            if (statusCode == 404) return null; // returning the null value from the 404 (user known)
            if (statusCode == 301)
            {
                // Working on the special information on the {id}_REPLACED error
            }

            handler.handleError(statusCode, res); // Handling all other response status codes

            string rawJson = res.rawJSON;
            bool malformedCheck = handler.IsMalformedResponse(rawJson);

            if (malformedCheck) throw new HttpError("The Airly API response get malformed or it is not fully normally");

            var convertedJson = ConvertJsonString(rawJson);
            bool jsonValidCheck = !string.IsNullOrEmpty(convertedJson.ToString());

            if (jsonValidCheck) return convertedJson;
            else if(!jsonValidCheck) throw new HttpError("The Airly API returned json is null or empty");

            return null; // Fallback value (when all other statments do not react with the response)
        }
    }

    // Handling all the RequestQueuers with the specified interaction first endpoints
    // The small List wrapper with the additional methods
    public class RequestQueueHandler : List<KeyValuePair<string, RequestQueuer>>
    {
        public int Handlers() => this.Count; // Getting all the active queuers
        public bool Inactive() => this.Count == 0; // Indicates if the QueuerHandler is inactive
        public bool RateLimited()
        {
            List<bool> limits = new List<bool>();
            foreach (var queuer in this) limits.Add(queuer.Value.RateLimited);

            List<bool> limited = limits.FindAll((e) => e == true);
            return limits.Count == limited.Count;
        }

        public RequestQueuer Get(string route)
        {
            var value = this.Find((e) => e.Key == route).Value;
            return value ?? null;
        }

        public void Set(string route, RequestQueuer queue)
        {
            KeyValuePair<string, RequestQueuer> element = new KeyValuePair<string, RequestQueuer>(route, queue);
            var checkedValue = Find((obj) => obj.Key == route);
            bool checkContains = checkedValue.Value != null;

            if (checkContains) {
                this.Remove(checkedValue);
                this.Add(element);
            }
            else this.Add(element);
        }

        // Reseting the queuer handler and destroying all key queuers
        public void Reset()
        {
            foreach (var val in this)
            {
                var queuer = val.Value ?? null;
                if (queuer == null || string.IsNullOrEmpty(val.Key)) continue;

                queuer.Zero(); // Detroying
                this.Remove(val); // Removing
            }
        }
    }
}