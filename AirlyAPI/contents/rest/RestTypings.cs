using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AirlyNet.Rest.Typings
{
    internal interface IRequest : IAirlyAuth, IRequestHeaders
    {
        RESTManager Rest { get; set; }
        Task<RawRestResponse> Send();
    }

    internal interface IAirlyAuth
    {
        public void SetLanguage(AirlyLanguage language);
        public void SetKey(string key);
    }

    internal interface IRequestHeaders
    {
        public void AddHeader(string key, string value);
        public void RemoveHeader(string key);
    }

    public enum RestRequestMethod
    {
        GET,
        POST,
        PATCH,
        DELETE,
        PUT
    }

    public class RequestOptions
    {
        public string[][] Query { get; set; }
        public string Body { get; set; }
        public bool Auth { get; set; } = true;
        public bool Versioned { get; set; } = true;
    }

    public class RestResponse
    {
        public JToken Json { get; set; }
        public string RawJson { get; set; }

        public HttpResponseMessage ResponseMessage { get; set; }
        public HttpResponseHeaders ResponseHeaders { get => ResponseMessage?.Headers; set { } }

        public DateTime ResponseTimestamp { get; set; }

        public Dictionary<string, object> InternalData = new Dictionary<string, object>();
    }

    public class RawRestResponse
    {
        public RawRestResponse(HttpResponseMessage response, string rawJson)
        {
            HttpResponse = response;
            RawJson = rawJson;
        }

        public HttpResponseMessage HttpResponse { get; set; }
        public string RawJson { get; set; }
    }
}
