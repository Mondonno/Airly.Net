using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using AirlyNet.Utilities;
using AirlyNet.Rest.Typings;
using AirlyNet.Common.Handling.Exceptions;
using AirlyNet.Common.Models;
using AirlyNet.Common;
using System.Net.Mime;

namespace AirlyNet.Rest.Api
{
    public static class RestRequestConstants
    {
        public static string ApiKeyHeader { get; } = "apikey";
        public static string AcceptLanguageHeader { get; } = "Accept-Language";
        public static string UserAgentHeader { get; } = "User-Agent";

        public static string HttpProtocol { get; } = "http";
        public static string HttpSecuredProtocol { get; } = "https";
    }

    public class DefaultRestRequest : IRequest, IDisposable
    {
        public RESTManager Rest { get; set; }
        public HttpMethod Method { get; set; }
        public RequestOptions RestOptions { get; set; }
        public AirlyConfiguration RestConfiguration { get; set; }
        private HttpClient HttpClient { get; set; }

        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>()
        {
            { "Accept", "*/*" },
            { "Connection", "keep-alive" },
            { "Cache-Control", "no-cache" },
            { "Accept-Encoding", "gzip" }
        };

        public Uri RequestUri { get; set; }
        public string RawUrl {
            set => RequestUri = new Uri(value);
        }
        public string RawMethod {
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
            Method = !string.IsNullOrEmpty(method) ? GetMethod(method) : GetMethod(HttpMethod.Get.Method);

            DefaultHeaders.Add(RestRequestConstants.UserAgentHeader, RestConfiguration.Agent);

            string parsedProtocol;
            if (RestConfiguration.Protocol == RestRequestProtocol.HTTPS)
                parsedProtocol = RestRequestConstants.HttpSecuredProtocol;
            else
                parsedProtocol = RestRequestConstants.HttpProtocol;

            string query = string.Empty;
            bool queryExists = options.Query != null && options.Query.Count != 0;

            if (queryExists)
                query = DecodeQuery(options.Query);

            RawUrl = $"{parsedProtocol}://{RestConfiguration.ApiDomain}/v{RestConfiguration.Version}/{EndPoint}{query}";

            RestHttpHandler restHttpHandler = new RestHttpHandler();
            HttpClient = new HttpClient(restHttpHandler, true);

            MergeHeaders(DefaultHeaders);
        }

        public async Task<RawRestResponse> Send()
        {
            double restTimeout = RestConfiguration.RestRequestTimeout;
            double requestTimeout = restTimeout == 0 ? TimeSpan.FromMinutes(1).TotalMilliseconds : restTimeout;

            HttpClient.Timeout = TimeSpan.FromMilliseconds(requestTimeout);
    
            HttpRequestMessage requestMessage = new(Method, RequestUri);
            if(RestOptions.Body != null)
            {
                StringContent content = new StringContent(RestOptions.Body.ToString(),
                    Encoding.UTF8, MediaTypeNames.Application.Json);
                requestMessage.Content = content;
            }

            HttpResponseMessage httpResponse = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None); // Error (capture)
            string httpContent = await httpResponse.Content.ReadAsStringAsync();
            
            httpResponse.Dispose();
            requestMessage.Dispose();

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
                throw new BaseException($"Aggregate Exception was thrown in RequestModule\n" + ag);
            }
            catch (Exception ex)
            {
                throw new HttpException($"{RequestUri}\n{ex.Message}");
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

        private string DecodeQuery(List<List<string>> queryObject)
        {
            string query = string.Empty;
            foreach (var segment in queryObject)
                query += string.Format("{0}={1}&", segment[0], segment[1]);

            query = query.EndsWith("&") ? query.Remove(query.Length - 1, 1) : query;

            if (query.StartsWith("?")) query = query.Remove(0, 1);

            string virtualHost = "http://127.0.0.1";
            Uri constructedQuery = new Uri(string.Format("{0}{1}{2}", virtualHost, "/",
                    string.IsNullOrEmpty(query) ? string.Empty // no need to parse the Query (eg. % === %25) (the Uri class do this for me)
                    : string.Format("?{0}", query // Replacing # and @ to the own URL code (Uri does not support # and @ in query)
                        .Replace("#", "%23")
                        .Replace("@", "%40"))));

            return constructedQuery.Query;
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

            ToggleHeader(RestRequestConstants.ApiKeyHeader, key);
        }

        public void SetMethod(string methodName) => RawMethod = methodName.Trim().ToUpper();

        public void SetLanguage(AirlyLanguage language) => SetLanguage(language.ToString().ToLower());

        public void SetLanguage(string language) => ToggleHeader(RestRequestConstants.AcceptLanguageHeader, language.ToLower());

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