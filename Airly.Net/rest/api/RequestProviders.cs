using System;
using System.Threading.Tasks;

using AirlyNet.Utilities;
using AirlyNet.Rest.Typings;
using AirlyNet.Models;

namespace AirlyNet.Rest
{
    // provides modified functions and wraps the original DeafultRestRequest
    public class RestRequest : RestRequestProvider
    {
        public RestRequest(RESTManager rest, string end)
            : base(rest, end, null, new()) { }

        public RestRequest(RESTManager rest, string end, RequestOptions options)
            : base(rest, end, null, options) { }

        public RestRequest(RESTManager rest, string end, string method, RequestOptions options = null)
            : base(rest, end, method, options) { }

        public RestRequest(RESTManager rest, string end, RestRequestMethod requestMethod, RequestOptions options = null)
            : base(rest, end, requestMethod.ToString(), options) { }

        public async Task<RawRestResponse> InvokeRequest(bool handle = true) => await SendAsync(handle);

        public async Task<T> InvokeRequest<T>(bool handle = true) => await SendAsync<T>(handle);
    }

    public class RestRequestProvider : IAirlyAuth
    {
        protected DefaultRestRequest RestRequest { get; set; }
        public RESTManager RestManager { get; set; }
        public Uri RequestUri { get => RestRequest.RequestUri; }
         
        public RestRequestProvider(RESTManager rest, string end, string method, RequestOptions options)
        {
            RestManager = rest;
            RestRequest = new DefaultRestRequest(rest, end, method, options);
        }

        protected async Task<RawRestResponse> SendAsync(bool handle = true) => await SendAsyncInternal(RestRequest, handle);

        protected async Task<T> SendAsync<T>(bool handle = true)
        {
            var httpResult = await SendAsync(handle);
            var parsed = new RestResponseParser<T>(httpResult.RawJson);
            return parsed.Deserializated;
        }

        protected async Task<RawRestResponse> SendAsyncInternal(DefaultRestRequest request, bool handle)
        {
            if (handle) return await request.SendAndHandle();
            else return await request.Send();
        }

        protected async Task<RawRestResponse> SendAsyncInternal(DefaultRestRequest request, string method, bool handle)
        {
            request.SetMethod(method.ToUpper()); // Setting method internally
            return await SendAsyncInternal(request, handle);
        }

        public void SetLanguage(AirlyLanguage language) => RestRequest.SetLanguage(language);

        public void SetKey(string key) => RestRequest.SetKey(key);

        public void SetMethod(string method) => RestRequest.SetMethod(method.ToUpper());

        public void SetRequest(DefaultRestRequest restRequest) => RestRequest = restRequest;
    }
}
