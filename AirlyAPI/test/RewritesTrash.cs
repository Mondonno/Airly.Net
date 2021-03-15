using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

using AirlyAPI;
using AirlyAPI.Rest;
using AirlyAPI.Utilities;
using AirlyAPI.Rest.Typings;


using System.Net;

namespace Tests
{
    public class RestRequestInvoker
    {
        public RESTManager Rest { get; set; }
        public RequestOptions RestOptions { get; set; }
        public RestRequest Request { get; set; }

        public string RequestUri { get; set; }

        public RestRequestInvoker(RESTManager rest, string url, RequestOptions options)
        {
            this.Rest = rest;
            this.RestOptions = options;
            this.RequestUri = url;

            this.Request = new RestRequest(rest, url, null, options);
        }

        public void GetAsync()
        {
            Request.RawMethod = "GET";
            Request.Send();
        }
    }

    public class RestHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Run(request.CreateResponse);
        }
    }

    interface IRequest
    {
        void SetKey(string key);
        void SetLanguage(AirlyLanguage language);

        string Send();
        Task<string> SendAsync();

        RESTManager Rest { get; set; }
    }

    [DnsPermission(System.Security.Permissions.SecurityAction.Assert)]
    [WebPermission(System.Security.Permissions.SecurityAction.Assert)]
    public class RestRequest : IRequest
    {
        public RESTManager Rest { get; set; }
        public HttpMethod Method { get; set; }
        public RequestOptions RestOptions { get; set; }
        public AirlyConfiguration RestConfiguration { get; set; }
        public HttpRequestMessage RequestMessage { get; set; }
        public Utils Util { get; set; }

        public Dictionary<string, string> DeafultHeaders = new Dictionary<string, string>()
        {
            { "Content-Type", "application/json" },
            { "Accept", "application/json" },
            { "Connection", "keep-alive" },
            { "Cache-Control", "no-cache" },
            { "Accept-Encoding", "gzip" }
        };

        public string RawMethod
        {
            set
            {
                Method = GetMethod(value);
            }
            get
            {
                return Method.Method;
            }
        }

        public string EndPoint { get; set; }

        public RestRequest(RESTManager rest, string end, string method, RequestOptions options)
        {
            this.Rest = rest;
            this.RestConfiguration = rest.Airly.Configuration;
            this.EndPoint = end;
            this.RestOptions = options;
            this.Method = !string.IsNullOrEmpty(method) ? GetMethod(method) : GetMethod("GET");

            DeafultHeaders.Add("User-Agent", this.RestConfiguration.Agent);
            MergeHeaders(DeafultHeaders);

            string url =
                this.RestConfiguration.Protocol +
                this.RestConfiguration.ApiDomain +
                this.EndPoint;

            bool queryExists = options.Query != null && options.Query.Length != 0;
            string query = string.Empty;

            if (queryExists)
            {
                foreach (var segment in options.Query)
                    query += string.Format("{0}={1}&", segment[0], segment[1]);

                query = query.EndsWith("&") ? query.Remove(query.Length - 1, 1) : query;
                query = Util.FormatQuery(query);
            }

            url = !string.IsNullOrEmpty(query) ? url + query : url;
            RequestMessage = new HttpRequestMessage(Method, url);
        }

        public string Send()
        {
            RestHttpHandler handler = new RestHttpHandler();
            HttpClient httpClient = new HttpClient(handler, true);

            HttpResponseMessage httpResponse = httpClient.SendAsync(RequestMessage).Result;
            string httpContent = httpResponse.Content.ReadAsStringAsync().Result;

            httpClient.Dispose();
            httpResponse.Dispose();

            return httpContent;
        }

        public async Task<string> SendAsync() => await Task.Run(Send);

        public void AddHeader(string key, string value) => RequestMessage.Headers.Add(key, value);
        public void RemoveHeader(string key) => RequestMessage.Headers.Remove(key);
        public void MergeHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
                RequestMessage.Headers.Add(header.Key, header.Value);
        }

        public HttpMethod GetMethod(string method)
        {
            HttpMethod httpMethod = HttpMethod.Get;
            string validatedMethod = method.Trim().Normalize().Replace(" ", "").ToUpper();

            try
            {
                httpMethod = new HttpMethod(validatedMethod);
            }
            catch (Exception) { }

            return httpMethod;
        }

        public void SetKey(string key)
        {
            if (!RestOptions.Auth) return;

            RemoveHeader("apikey");
            AddHeader("apikey", key);
        }

        public void SetLanguage(AirlyLanguage language)
        {
            RemoveHeader("Accept-Language");
            AddHeader("Accept-Language", language.ToString().ToLower());
        }
    }
}