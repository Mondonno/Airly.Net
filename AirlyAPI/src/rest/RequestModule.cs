using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using System.Text;

using AirlyAPI.Handling;
using AirlyAPI.Utilities;
using System.Runtime.ExceptionServices;
using System.Diagnostics;

namespace AirlyAPI.Rest
{
    // todo przepisac RestRequest na reuseable (mozna wyslac kilka tych samych requestów przez jedna klase RestRequest) czyli inaczej zrobic HttpClient constantem a HttpRequest Message zdjąć z constanta
    // todo zfixowac SendAndHandle() poniewaz robi buga jesli wywola sie asynchronicznie i rzuci w srodku exceptionem
    // todo zmienic usage z DeafultRestRequest na RestRequest który implementuje RestRequestProvider'a który wrapuje klase deafult rest request
    
    // The pack of all required methods to interact with API
    public class RestRequest : RestRequestProvider
    {
        public RestRequest(RESTManager rest, string end, string[][] query) : base(rest, end, query) { }
        public RestRequest(RESTManager rest, string end, string[][] query, bool versioned) : this(rest, end, query)
        {
            base.Request.RestOptions.Versioned = versioned;
        }
        public RestRequest(RESTManager rest, string end, RequestOptions options) : base(rest, end)
        {
            base.Request.RestOptions = options;
        }

        public async Task<RawResponse> InvokeRequest(bool handle = true)
        {
            return await base.SendAsync(handle);
        }
        public async Task<T> InvokeRequest<T>(bool handle = true)
        {
            return await base.SendAsync<T>(handle);
        }
    }

    public class RestHttpHandler : HttpClientHandler
    {
        public RestHttpHandler() : base()
        {
            AutomaticDecompression = DecompressionMethods.GZip;
            AllowAutoRedirect = true;
            UseCookies = false;
            UseProxy = false;
        }
    }

    interface IRequest
    {
        void SetKey(string key);
        void SetLanguage(AirlyLanguage language);

        Task<RawResponse> Send();

        RESTManager Rest { get; set; }
    }

    [DnsPermission(SecurityAction.Assert)]
    [WebPermission(SecurityAction.Assert)]
    public class DeafultRestRequest : IRequest, IDisposable
    {
        public RESTManager Rest { get; set; }
        public HttpMethod Method { get; set; }
        public RequestOptions RestOptions { get; set; }
        public AirlyConfiguration RestConfiguration { get; set; }
        public HttpRequestMessage RequestMessage { get; set; }
        public Utils Util { get; set; } = new Utils();

        public Dictionary<string, string> DeafultHeaders = new Dictionary<string, string>()
        {
            { "Accept", "*/*" },
            { "Connection", "keep-alive" },
            { "Cache-Control", "no-cache" },
            { "Accept-Encoding", "gzip" }
        };

        public Uri RequestUri { get => RequestMessage.RequestUri; }
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

        protected bool _isDisposed;
        public string EndPoint { get; set; }

        public DeafultRestRequest(RESTManager rest, string end, string method, RequestOptions options)
        {
            this.Rest = rest;
            this.RestConfiguration = rest.Airly.Configuration;
            this.EndPoint = end;
            this.RestOptions = options;
            this.Method = !string.IsNullOrEmpty(method) ? GetMethod(method) : GetMethod("GET");

            DeafultHeaders.Add("User-Agent", this.RestConfiguration.Agent ?? "Airly API C#");

            string url =
                this.RestConfiguration.Protocol + "://" +
                this.RestConfiguration.ApiDomain + Utils.GetVersion(this.RestConfiguration.Version, true) +
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

            RequestMessage = new HttpRequestMessage(Method, new Uri(url));

            MergeHeaders(DeafultHeaders);
        }

        public async Task<RawResponse> Send()
        {
            double restTimeout = this.RestConfiguration.RestRequestTimeout;
            double requestTimeout = restTimeout == 0 ? 60000 : restTimeout;

            RestHttpHandler handler = new RestHttpHandler();
            HttpClient httpClient = new HttpClient(handler, true) { Timeout = TimeSpan.FromMilliseconds(requestTimeout) };

            if(this.RestOptions.Body != null)
            {
                StringContent content = new StringContent(this.RestOptions.Body.ToString(), Encoding.UTF8, "application/json");
                RequestMessage.Content = content;
            }

            HttpResponseMessage httpResponse = await httpClient.SendAsync(RequestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None); // Error (capture)
            string httpContent = await httpResponse.Content.ReadAsStringAsync();
            
            httpClient.Dispose();
            httpResponse.Dispose();

            RequestMessage.Dispose();
            Refresh();

            _ = this.RestOptions.Body != null ? RequestMessage.Content = null : null;

            RawResponse restResponse = new RawResponse(httpResponse, httpContent);

            return restResponse;
        }
        public async Task<RawResponse> SendAndHandle()
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
                throw new HttpError($"{this.RequestMessage.RequestUri}\n{ex.Message}");
            }
        }

        public void Refresh() {
            RequestMessage = new HttpRequestMessage(RequestMessage.Method, RequestMessage.RequestUri);
            MergeHeaders(DeafultHeaders);
        }
        public void AddHeader(string key, string value) => RequestMessage.Headers.Add(key, value);
        public void RemoveHeader(string key) => RequestMessage.Headers.Remove(key);
        public void MergeHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                RemoveHeader(header.Key);
                AddHeader(header.Key, header.Value);
            }
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

        public void SetLanguage(AirlyLanguage language) => SetLanguage(language.ToString().ToLower());
        public void SetLanguage(string language)
        {
            RemoveHeader("Accept-Language");
            AddHeader("Accept-Language", language);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                RequestMessage.Dispose();
                this._isDisposed = true;
            }
        }
    }
}