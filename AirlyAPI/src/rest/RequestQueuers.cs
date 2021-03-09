using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

using AirlyAPI.Utilities;
using AirlyAPI.Rest;

namespace AirlyAPI.Handling
{
    // Simple wrapper for semaphore slim to handle threads in requests
    public class Waiter : SemaphoreSlim
    {
        public Waiter() : base(1, 1){} // Limiting to one action per push and one thread (so user can make a lot of threads and the thread does not have childs)

        // Shortcuts
        public int RemainingTasks() => this.CurrentCount;
        public void Destroy() => this.Dispose(true);
    }

    public class RequestQueuer
    {
        public Waiter Waiter { get; private set; }
        public RESTManager Manager { get; private set; }

        public bool RateLimited { get; private set; } = false; // The ratelimit error can be handled by user so thats why this variable exists
        public bool Inactive { get => Waiter.RemainingTasks() == 0 && !this.RateLimited; }

        private CancellationToken _cancellationToken { get; set; } = CancellationToken.None; // cs-unused

        public RequestQueuer(RESTManager manager)
        {
            this.Manager = manager;
            this.Waiter = new Waiter();
        }

        public async Task<AirlyResponse> Push(DeafultRestRequest request)
        {
            await Waiter.WaitAsync();

            try { return await this.Make(request); }
            finally { Waiter.Release(); }
        }

        public void Zero()
        {
            this.Waiter.Destroy();
            this.RateLimited = false;
        }

        private JToken ConvertJsonString(RawResponse res) => ConvertJsonString(res.rawJSON);
        private JToken ConvertJsonString(string json)
        {
            JToken token = JsonParser.ParseJson(json);
            return token;
        }

        private async Task<AirlyResponse> Make(DeafultRestRequest request)
        {
            Handler handler = new Handler();
            RawResponse res;

            try { res = await request.SendAndHandle(); }
            catch(Exception ex) { throw ex; };

            if (res == null || string.IsNullOrEmpty(res.rawJSON)) throw new HttpError("Can not resolve the Airly api response");
            if (this.RateLimited)
            {
                var details = new RateLimitInfo(res.response);
                if (details.IsRateLimited) handler.MakeRateLimitError(res, new Utils(), null);
                else {
                    this.RateLimited = false;
                    System.Diagnostics.Debug.WriteLine("The ratelimited is detected but it is not authenticated by headers");
                }
            }

            HttpResponseHeaders headers = res.response.Headers;
            int statusCode = (int) res.response.StatusCode;
            string rawDate = new Utils().GetHeader(headers, "Date");

            AirlyResponse constructedResponse = new AirlyResponse(ConvertJsonString(res), headers, res.rawJSON, !string.IsNullOrEmpty(rawDate) ? DateTime.Parse(rawDate) : DateTime.Now);

            if (statusCode == 200 || res.response.IsSuccessStatusCode) return constructedResponse; // Nothing wrong with request & response, returning
            if (statusCode == 404)
            {
                var notFoundEnum = this.Manager.Airly.Configuration.NotFoundHandling;
                if (notFoundEnum.Equals(AirlyNotFoundHandling.Null)) return null; // returning the null value from the 404 (user known)
                else if (notFoundEnum.Equals(AirlyNotFoundHandling.Error)) throw new HttpError($"The content for {request.RequestUri} was not found"); // or throwing the error on the AirlyNotFound handling setting
            }
            if (statusCode == 301)
            {
                ErrorModel parsedError = handler.GetErrorFromJson(ConvertJsonString(res.rawJSON));
                int? succesor = parsedError.succesor;

                if (succesor == null) throw new HttpError("[301] [INSTALLATION_REPLACED] Installation get replaced and new succesor was not found");
                else if (succesor != null)
                {


                } else throw new HttpError("[301] [UNHANDLED]");

                // Working on the special information on the {id}_REPLACED (INSTALLATION_REPLACED) errors
            }

            handler.HandleError(statusCode, res);

            string rawJson = res.rawJSON;
            bool malformedCheck = handler.IsMalformedResponse(rawJson);

            if (malformedCheck)
                throw new HttpError("The Airly API response get malformed or it is not fully normally");

            var convertedJson = ConvertJsonString(rawJson);
            bool jsonValidCheck = !string.IsNullOrEmpty(convertedJson.ToString());
            
            if (jsonValidCheck) return constructedResponse;
            else if(!jsonValidCheck)
                throw new HttpError("The Airly API returned json is null or empty");

            return null; // Fallback value (when all other statments do not react with the response)
        }
    }

    public class RequestQueueManager : Dictionary<string, RequestQueuer>, IDisposable // Handler for all avaible routes on the API Manager
    {
        public RequestQueueManager() { }

        public int Handlers() => Count;
        public bool Inactive() => Count == 0;
        public bool RateLimited()
        {
            if (this.Count == 0) return false;

            List<bool> limits = new List<bool>(this.Values.Select((e, b) => e.RateLimited));
            int limited = limits.FindAll((e) => e == true).Count;

            return limits.Count == limited;
        }

        public RequestQueuer Get(string route)
        {
            TryGetValue(route, out RequestQueuer queuer);
            return queuer ?? null;
        }
        public void Set(string route, RequestQueuer queuer)
        {
            bool contains = base.ContainsKey(route);
            if (contains) Remove(route);
            Add(route, queuer);
        }
        public void Dispose()
        {
            foreach (var avaibleQueuer in this)    
            {
                var queuer = avaibleQueuer.Value ?? null;
                if (queuer == null || string.IsNullOrEmpty(avaibleQueuer.Key)) continue;

                queuer.Zero();
            }
            Clear();
        }
    }

    public class RequestQueueHandler
    {
        private RequestQueueManager QueueManager = new RequestQueueManager();
        private RESTManager RestManager { get; set; }

        // shortcuts
        public bool Inactive() => QueueManager.Inactive();
        public bool RateLimited() => QueueManager.RateLimited();
        public int Queuers() => QueueManager.Handlers();

        public RequestQueueHandler(RESTManager rest)
        {
            this.RestManager = rest;
        }

        public Task<AirlyResponse> Queue(string route, DeafultRestRequest request)
        {
            RequestQueuer handler = QueueManager.Get(route);

            if (handler == null)
            {
                handler = new RequestQueuer(RestManager);
                QueueManager.Set(route, handler);
            }

            return handler.Push(request);
        }
        public void UnQueue(string route)
        {
            QueueManager.TryGetValue(route, out RequestQueuer queuer);
            if (queuer == null) return;

            queuer.Zero();
            QueueManager.Remove(route);
        }

        public void Reset() => QueueManager.Dispose();
    }
}