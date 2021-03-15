﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using System.Text;

using AirlyAPI.Handling;
using AirlyAPI.Utilities;
using AirlyAPI.Rest.Typings;
using AirlyAPI.Handling.Errors;

namespace AirlyAPI.Rest
{
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

    [DnsPermission(SecurityAction.Assert)]
    [WebPermission(SecurityAction.Assert)]
    public class DeafultRestRequest : IRequest, IDisposable, IAirlyAuth
    {
        public RESTManager Rest { get; set; }
        public HttpMethod Method { get; set; }
        public RequestOptions RestOptions { get; set; }
        public AirlyConfiguration RestConfiguration { get; set; }
        public HttpClient HttpClient { get; set; }
        public Utils Util { get; set; } = new Utils();

        public Dictionary<string, string> DeafultHeaders = new Dictionary<string, string>()
        {
            { "Accept", "*/*" },
            { "Connection", "keep-alive" },
            { "Cache-Control", "no-cache" },
            { "Accept-Encoding", "gzip" }
        };

        public Uri RequestUri { get; set; }
        private string RawUrl {
            get => RawUrl;
            set => RequestUri = new Uri(value);
        }
        public string RawMethod {
            set => Method = GetMethod(value);
            get => Method.Method;
        }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get => HttpClient.DefaultRequestHeaders; }

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
            RawUrl = url;

            RestHttpHandler restHttpHandler = new RestHttpHandler();
            HttpClient = new HttpClient(restHttpHandler, true);

            MergeHeaders(DeafultHeaders);
        }

        public async Task<RawRestResponse> Send()
        {
            double restTimeout = this.RestConfiguration.RestRequestTimeout;
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

        public void AddHeader(string key, string value) => HttpClient.DefaultRequestHeaders.Add(key, value);
        public void RemoveHeader(string key) => HttpClient.DefaultRequestHeaders.Remove(key);
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
                HttpClient.Dispose();
                this._isDisposed = true;
            }
        }
    }
}