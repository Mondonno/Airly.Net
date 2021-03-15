using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AirlyAPI.Rest.Typings
{
    internal interface IRequest : IAirlyAuth
    {
        RESTManager Rest { get; set; }
        Task<RawRestResponse> Send();
    }

    internal interface IAirlyAuth
    {
        public void SetLanguage(AirlyLanguage language);
        public void SetKey(string key);
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
    }

    public class RawRestResponse
    {
        public RawRestResponse(HttpResponseMessage response, string rawJSON)
        {
            this.HttpResponse = response;
            this.RawJson = rawJSON;
        }

        public HttpResponseMessage HttpResponse { get; set; }
        public string RawJson { get; set; }
    }

}
