// Now no in use, prepared to v1.5

using System;
using System.Threading.Tasks;

using AirlyAPI.Utilities;
using AirlyAPI.Rest;

namespace AirlyAPI.Rest
{
    // Simple enum what serializing the RequestType
    public enum AirlyRequestType
    {
        GET,
        POST,
        DELETE,
        PUT,
        PATCH
    }

    // Simple class that providing the RestRequests handlings, and automatic requests methods, also custom constructors
    public class RestRequestProvider
    {
        public DeafultRestRequest Request { get; set; }
        protected RESTManager Manager { get; set; }

        public RestRequestProvider(RESTManager rest, string end)
        {
            Request = new DeafultRestRequest(rest, end, "GET", new RequestOptions());
        }
        public RestRequestProvider(RESTManager rest, string end, string method) : this(rest, end)
        {
            Request.RawMethod = method.ToUpper();
        }
        public RestRequestProvider(RESTManager rest, string end, string[][] query) : this(rest, end)
        {
            Request.RestOptions.Query = query;
        }
        public RestRequestProvider(RESTManager rest, string end, string[] body) : this(rest, end)
        {
            Request.RestOptions = new RequestOptions() {
                Auth = Request.RestOptions.Auth,
                Body = ArrayUtil.JoinArray(body, ""),
                Query = Request.RestOptions.Query
            };
        }
        public RestRequestProvider(RESTManager rest, string end, bool versioned) : this(rest, end)
        {
            Request.RestOptions.Versioned = versioned;
        }
        public RestRequestProvider(RESTManager rest, string end, AirlyRequestType method) : this(rest, end, method.ToString()) { }
        
        public async Task<RawResponse> SendAsync(bool handle = true) => await SendAsyncInternal(Request, handle);
        public async Task<T> SendAsync<T>(bool handle = true)
        {
            var httpResult = await SendAsync(handle);
            var parsed = new RestResponseParser<T>(httpResult.rawJSON);
            return parsed.Deserializated;
        } 

        protected async Task<RawResponse> SendAsyncInternal(DeafultRestRequest request, bool handle)
        {
            if (handle) return await request.SendAndHandle();
            else return await request.Send();
        }
        protected async Task<RawResponse> SendAsyncInternal(DeafultRestRequest request, string method, bool handle)
        {
            request.RawMethod = method.ToUpper(); // Setting method internally
            return await SendAsyncInternal(request, handle);
        }
        
        public void SetLanguage(AirlyLanguage language) => Request.SetLanguage(language);
        public void SetMethod(string method) => Request.RawMethod = method.ToUpper();
    }
}
