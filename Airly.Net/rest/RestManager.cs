using System;
using System.Threading.Tasks;

using AirlyNet.Utilities;
using AirlyNet.Rest.Typings;
using AirlyNet.Common.Handling.Exceptions;
using AirlyNet.Common;
using AirlyNet.Queue;

namespace AirlyNet.Rest
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

        public string Auth { get => string.IsNullOrEmpty(ApiKey) ? throw new InvalidApiKeyException("Provided api key is empty") : ApiKey; }
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
            var responseData = new RestResponseParser<T>(httpResponse != null ? httpResponse.RawJson : null);

            if (responseData != null) return responseData.Deserializated;
            else return default;
        }

        public async Task<T> Request<T>(string end, string method = null, dynamic query = null, RequestOptions options = null)
        {
            RequestOptions requestOptions = options ?? (new());
            if (query != null) requestOptions.Query = Utils.ParseQuery(query);

            return await Request<T>(end, method, requestOptions);
        }

        public void Dispose() => Handlers.Dispose();
    }
}
