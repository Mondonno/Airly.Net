using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using System.Text;

using AirlyNet.Utilities;
using AirlyNet.Rest.Typings;
using AirlyNet.Handling.Errors;
using AirlyNet.Models;

namespace AirlyNet.Rest
{
    [DnsPermission(SecurityAction.Assert)]
    [WebPermission(SecurityAction.Assert)]
    public class DefaultRestRequest : IRequest, IDisposable
    {
        public RESTManager Rest { get; set; }
        public HttpMethod Method { get; set; }
        public RequestOptions RestOptions { get; set; }
        public AirlyConfiguration RestConfiguration { get; set; }
        private HttpClient HttpClient { get; set; }
        public Utils Util { get; set; } = new Utils();

        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>()
        {
            { "Accept", "*/*" },
            { "Connection", "keep-alive" },
            { "Cache-Control", "no-cache" },
            { "Accept-Encoding", "gzip" }
        };

        public Uri RequestUri { get; set; }
        private string RawUrl {
            get => RawUrl;
            set =>  RequestUri = new Uri(value);
        }
        private string RawMethod {
            set => Method = GetMethod(value);
        }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get => HttpClient.DefaultRequestHeaders; }

        protected bool _isDisposed;
        public string EndPoint { get; set; }

        public DefaultRestRequest(RESTManager rest, string end, string method, RequestOptions options)
        {
            Rest = rest;
            RestConfiguration = rest.Airly.Configuration;
            EndPoint = end;
            RestOptions = options;
            Method = !string.IsNullOrEmpty(method) ? GetMethod(method) : GetMethod("GET");

            DefaultHeaders.Add("User-Agent", RestConfiguration.Agent);

            string url =
                $"{RestConfiguration.Protocol ?? "https"}://" +
                RestConfiguration.ApiDomain + Utils.GetVersion(RestConfiguration.Version, true) +
                EndPoint;

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
            RawUrl = url;

            RestHttpHandler restHttpHandler = new RestHttpHandler();
            HttpClient = new HttpClient(restHttpHandler, true);

            MergeHeaders(DefaultHeaders);
        }

        public async Task<RawRestResponse> Send()
        {
            double restTimeout = RestConfiguration.RestRequestTimeout;
            double requestTimeout = restTimeout == 0 ? 60000 : restTimeout;

            HttpClient.Timeout = TimeSpan.FromMilliseconds(requestTimeout);

            HttpRequestMessage RequestMessage = new(Method, RequestUri);
            if(RestOptions.Body != null)
            {
                StringContent content = new StringContent(RestOptions.Body.ToString(), Encoding.UTF8, "application/json");
                RequestMessage.Content = content;
            }

            HttpResponseMessage httpResponse = await HttpClient.SendAsync(RequestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None); // Error (capture)
            string httpContent = await httpResponse.Content.ReadAsStringAsync();
            
            httpResponse.Dispose();
            RequestMessage.Dispose();

            RawRestResponse restResponse = new(httpResponse, httpContent);
            return restResponse;
        }
        public async Task<RawRestResponse> SendAndHandle()
        {
            try
            {
                return await Send();
            }
            catch (AggregateException ag)
            {
                throw new Exception($"New agregated error (potential thread lock)\n{Utils.GetInners(ag)}");
            }
            catch (Exception ex)
            {
                throw new HttpError($"{RequestUri}\n{ex.Message}");
            }
        }

        public void ToggleHeader(string key, string value)
        {
            RemoveHeader(key);
            AddHeader(key, value);
        }
        public void AddHeader(string key, string value) => HttpClient.DefaultRequestHeaders.Add(key, value);
        public void RemoveHeader(string key) => HttpClient.DefaultRequestHeaders.Remove(key);
        public void MergeHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
                ToggleHeader(header.Key, header.Value);
        }

        private HttpMethod GetMethod(string method)
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

            ToggleHeader("apikey", key);
        }

        public void SetMethod(string methodName) => RawMethod = methodName.Trim().ToUpper();
        public void SetLanguage(AirlyLanguage language) => SetLanguage(language.ToString().ToLower());
        public void SetLanguage(string language) => ToggleHeader("Accept-Language", language.ToLower());

        public void Dispose()
        {
            if (!_isDisposed)
            {
                HttpClient.Dispose();
                _isDisposed = true;
            }
        }
    }

    public class RestHttpHandler : HttpClientHandler
    {
        public RestHttpHandler(bool proxy = false, bool cookies = false) : base()
        {
            AutomaticDecompression = DecompressionMethods.GZip;
            AllowAutoRedirect = true;
            UseCookies = cookies;
            UseProxy = proxy;
        }
    }
}