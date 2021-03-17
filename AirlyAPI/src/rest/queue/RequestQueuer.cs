﻿using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;

using AirlyAPI.Utilities;
using AirlyAPI.Rest.Typings;
using AirlyAPI.Rest;
using AirlyAPI.Handling.Errors;
using System.Net.Http;

namespace AirlyAPI.Handling
{
    public class Waiter : SemaphoreSlim // Deafult SemaphoreSlim configuration
    {
        public Waiter() : base(1, 1) { }

        // Shortcuts
        public int RemainingTasks() => this.CurrentCount;
        public void Destroy() => this.Dispose(true);
    }

    public class RequestQueuer : IDisposable
    {
        public Waiter Waiter { get; private set; }
        public RESTManager Manager { get; private set; }

        public bool RateLimited { get; private set; } = false; // The ratelimit error can be handled by user so thats why this variable exists
        public bool Inactive { get => Waiter.RemainingTasks() == 0 && !this.RateLimited; }

        public RequestQueuer(RESTManager manager)
        {
            Manager = manager;
            Waiter = new Waiter();
        }

        public async Task<RestResponse> Push(RestRequest request)
        {
            await Waiter.WaitAsync();

            try { return await Make(request); }
            finally { Waiter.Release(); }
        }

        private void ThrowIfJsonError(HttpResponseMessage httpResponse,string responseJson)
        {
            ErrorDeserializer errorDeserializer = new(httpResponse, responseJson);
            ErrorModel deserializtedError = errorDeserializer.Deserialize();

            if (!deserializtedError.HaveError) return;
            else throw new AirlyError($"The api resolved the Json Error:\n{deserializtedError.ErrorDetails}");
        }

        private JToken ConvertJsonString(RawRestResponse res) => ConvertJsonString(res.RawJson);
        private JToken ConvertJsonString(string json)
        {
            JToken token = JsonParser.ParseJson(json);
            return token;
        }

        private async Task<RestResponse> Make(RestRequest request)
        {
            Utils utils = new();
            RawRestResponse res;

            try { res = await request.InvokeRequest(true); }
            catch (Exception ex) { throw ex; };

            if (res == null || string.IsNullOrEmpty(res.RawJson)) throw new HttpError("Can not resolve the Airly api response");
            if (this.RateLimited)
            {
                var details = new RateLimitInfo(res.HttpResponse);
                if (details.IsRateLimited) RateLimitThrower.MakeRateLimitError(res, utils, null);
                else {
                    this.RateLimited = false;
                    System.Diagnostics.
                        Debug.WriteLine("The ratelimited is detected but it is not authenticated by headers");
                }
            }

            Handler handler = new Handler(res.HttpResponse);
            HttpResponseHeaders headers = res.HttpResponse.Headers;

            int statusCode = (int) res.HttpResponse.StatusCode;
            string rawDate = utils.GetHeader(headers, "Date");

            RestResponse constructedResponse = new()
            {
                Json = ConvertJsonString(res),
                RawJson = res.RawJson,
                ResponseTimestamp = !string.IsNullOrEmpty(rawDate) ? DateTime.Parse(rawDate) : DateTime.Now,
                ResponseMessage = res.HttpResponse
            };

            if (statusCode == 200 || res.HttpResponse.IsSuccessStatusCode) return constructedResponse; // Nothing wrong with request & response, returning
            if (statusCode == 404)
            {
                var notFound = Manager.Airly.Configuration.NotFoundHandling;

                if (notFound != AirlyNotFoundHandling.Null) throw new HttpError($"The content for {request.RequestUri}\nWas not found"); // or throwing the error on the AirlyNotFound handling setting
                else return null; // returning the null value from the 404 (user known)
            }
            if (statusCode == 301)
            {
                ErrorDeserializer errorDeserializer = new(res.HttpResponse, res.RawJson);
                ErrorModel parsedError = errorDeserializer.Deserialize();
                string succesor = parsedError.Succesor;

                if (succesor == null) throw new HttpError("[301] [INSTALLATION_REPLACED] Installation get replaced and new succesor was not found");
                else if (succesor != null) { throw new NotImplementedException(); }
                else throw new HttpError("[301] [UNHANDLED]");

                // Working on the special information on the {id}_REPLACED (INSTALLATION_REPLACED) errors
            }

            string rawJson = res.RawJson;

            handler.HandleResponseCode();
            handler.Refersh(rawJson);

            bool malformedCheck = handler.JsonHandler.IsMalformedResponse();

            if (malformedCheck)
                throw new HttpError("The Airly API response get malformed or it is not fully normally");

            var convertedJson = ConvertJsonString(rawJson);
            bool jsonValidCheck = !string.IsNullOrEmpty(convertedJson.ToString());
            
            if (jsonValidCheck) return constructedResponse;
            else if(!jsonValidCheck)
                throw new HttpError("The Airly API returned json is null or empty");

            ThrowIfJsonError(res.HttpResponse, res.RawJson);
            return null; // Fallback value (when all other statments do not react with the response)
        }

        public void Dispose()
        {
            Waiter.Destroy();
            RateLimited = false;
        }
    }
}