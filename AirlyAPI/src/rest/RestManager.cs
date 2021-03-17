using System;
using System.Threading.Tasks;

using AirlyAPI.Handling;
using AirlyAPI.Utilities;
using AirlyAPI.Rest.Typings;
using AirlyAPI.Handling.Errors;

namespace AirlyAPI.Rest
{
    public class RESTManager : IDisposable
    {
        public Airly Airly { get; set; }
        private RequestQueueHandler Handlers { get; set; }

        private string ApiKey { get; set; }
        public AirlyLanguage Language { get; set; }

        public RESTManager(Airly airly) : this(airly, airly.ApiKey) { }
        public RESTManager(Airly airly, string apiKey)
        {
            ApiKey = apiKey;
            Airly = airly;
            Handlers = new RequestQueueHandler(this);
        }

        public string Auth { get => string.IsNullOrEmpty(ApiKey) ? throw new InvalidApiKeyError("Provided api key is empty") : ApiKey; }
        public bool RateLimited { get => Handlers.Queuers() >= 1 && Handlers.RateLimited(); }

        public string Endpoint { get => Airly.Configuration.ApiDomain; }
        public string Cdn { get => Airly.Configuration.Cdn; }

        private void SetAirlyPreferedLanguage(Airly airly) => Language = airly.Language;
        public Task<RestResponse> Request(string end, string method = null, RequestOptions options = null)
        {
            if (options == null) options = new RequestOptions();
            if (Auth != null) options.Auth = true;

            string identifyRoute = Utils.GetRoute(end);
            RestRequest request = new RestRequest(this, end, method, options);

            request.SetKey(Auth);
            request.SetLanguage(Language);

            return Handlers.Queue(identifyRoute, request);
        }

        public async Task<T> Request<T>(string end, string method = null, RequestOptions options = null)
        {
            RestResponse httpResponse = await Request(end, method, options);
            return new RestResponseParser<T>(httpResponse.RawJson).Deserializated;
        }
        public async Task<T> Request<T>(string end, string method = null, dynamic query = null, RequestOptions options = null)
        {
            Utils util = new();
            RequestOptions requestOptions = options != null ? options : new();
            if (query != null) requestOptions.Query = util.ParseQuery(query);

            return await Request<T>(end, method, requestOptions);
        }

        public void Dispose() => Handlers.Dispose();
    }
}
