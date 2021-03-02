﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace AirlyAPI
{
    [DnsPermission(System.Security.Permissions.SecurityAction.Assert)]
    [WebPermission(System.Security.Permissions.SecurityAction.Assert)]
    /// <summary>Raw request module for Airly API Wrapper *(Do not use this in yours project, just check API manager)</summary>
    public class RequestModule
    {
        private static int VERSION { get; set; } = 2;
        private static string API_DOMAIN { get; set; } = $"airapi.{Utils.domain}";

        private string LANGUAGE_CODE { get; set; } = "en"; // Deafult language code is the english
        private protected string API_KEY { get; set; } = null;
       
        public string API_URL { get; set; } = string.Format("https://{0}/v{1}/", API_DOMAIN, VERSION);
        public string API_KEY_HEADER_NAME { get; set; } = "apikey";

        private Utils moduleUtil { get; set; } = new Utils();

        public object options { get; set; }
        public object deafultHeaders { get; set; }

        public string method, Agent, endPoint;
        public string path { get; set; }

        /// <summary>Raw request module for Airly API Wrapper <i>(Do not use this in yours project, just check API manager)</i></summary>
        public RequestModule(string endPoint, string method, RequestOptions options)
        {
            this.options = options;
            this.method = method;
            this.endPoint = endPoint;

            // Decalaring all deafult headers
            string[] agent = { "User-Agent", "Airly-C#-Wrapper" };
            string[] connection = { "Connection", "keep-alive" };
            string[] cacheControl = { "Cache-Control", "no-cache" };
            string[] encoding = { "Accept-Encoding", "gzip" }; // Added encoding
            string[] accept = { "Accept", "application/json" };

            deafultHeaders = new string[][] {
                agent,
                connection,
                cacheControl,
                encoding,
                accept
            };

            string queryString = "";
            string finalQuery = "";

            if (options.query != null && options.query.Length > 0)
            {
                for (int i = 0; i < options.query.Length; i++)
                {
                    string key = options.query[i][0];
                    string value = options.query[i][1];

                    queryString += string.Format("{0}={1}&", key, value);
                    if (i >= (options.query.Length - 1)) queryString = queryString.Substring(0, queryString.Length - 1);
                }

                finalQuery = moduleUtil.formatQuery(queryString);
            }

            string Query = finalQuery;

            // Eg. Retruns from https://example.com/?test=Absurdal_test --> /?test=Absurdal_test
            this.path = $"{endPoint}{(!string.IsNullOrEmpty(Query) ? Query : "")}";
        }

        /// <summary>
        /// Making a request to the Airly API.
        /// </summary>
        /// <param name="customHeaders">The custom headers for the request</param>
        /// <param name="body">Obosolete (actually the requester supports al lot of methods but only streaming the text (json))</param>
        /// <returns></returns>
        public async Task<AirlyResponse> MakeRequest(string[][] customHeaders = null, string body = null)
        {
            if (string.IsNullOrEmpty(API_KEY) || string.IsNullOrEmpty(API_KEY.Replace(" ", ""))) throw new AirlyError(new HttpError("The provided airly api key is empty"));
            if(customHeaders == null) customHeaders = new string[0][];

            // Initializing the request headers used in initializing the HttpClient
            string[][] requestHeaders;

            // The request response timeout
            double timeout = 60000;

            string[] apiKey = { API_KEY_HEADER_NAME, "key" };
            string[] contentType = { "Content-Type", "application/json" };
            string[] language = { "Accept-Language", LANGUAGE_CODE ?? "en" };
            // Accept Languages:
            // * pl
            // * en

            moduleUtil.ArrayPush(ref customHeaders, language);

            if (!string.IsNullOrEmpty(body)) moduleUtil.ArrayPush(ref customHeaders, contentType);
            if (string.IsNullOrEmpty(apiKey[1])) apiKey[1] = API_KEY.ToString();

            // when the custom headers exists we assigning the on array to another
            string[][] copiedDeafultHeaders = (string[][]) ((string[][]) deafultHeaders).Clone();
            string[] customKey = Array.Find(customHeaders, (key) => key[0] == API_KEY_HEADER_NAME);

            if (customKey != null) apiKey[1] = string.Format("{0}", customKey);
            if ((customHeaders.Length - 1) >= 0) {
                customHeaders = moduleUtil.assignArray(copiedDeafultHeaders, customHeaders);
            }
            else customHeaders = copiedDeafultHeaders;
            requestHeaders = customHeaders;

            string RequestWebUrl = string.Format("{0}{1}", API_URL, path);
            
            Uri RequestUri = new Uri(RequestWebUrl);
            AuthenticationHeaderValue airlyAuthentication = new AuthenticationHeaderValue(apiKey[0], apiKey[1]); // todo wywalic to poniewaz autoryzacja jest w headerach :)

            HttpRequestMessage RequestParams = new HttpRequestMessage();
            HttpClient RequestClient = new HttpClient();

            SetHeaders(ref RequestClient, requestHeaders);

            RequestClient.CancelPendingRequests(); // Prevenitng infinity thread loop (if user does not await and one request must kill another)
            RequestParams.Method = GetMethod(this.method);

            RequestClient.BaseAddress = RequestUri;
            RequestClient.DefaultRequestHeaders.Authorization = airlyAuthentication;

            RequestClient.Timeout = TimeSpan.FromMilliseconds(timeout);

            HttpResponseMessage response;
            try 
            {
                 response = await RequestClient.GetAsync(RequestUri);
            }
            catch (Exception ex)
            {
                HttpError httpError = new HttpError(string.Format("{0}\n{1}", RequestWebUrl, ex.Message.ToString()));
                httpError.Data.Add("Connected", false);

                throw httpError;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            RequestClient.Dispose();

            JToken convertedJSON = JsonParser.ParseJson(responseBody);

            AirlyResponse airlyResponse = new AirlyResponse(
                convertedJSON,
                response.Headers,
                responseBody,
                DateTime.Now // Only for the raw requests without handlers
            );
            response.Dispose();

            return airlyResponse;
        }

        public void setKey(string key)
        {
            API_KEY = key;

            string[][] copiedHeaders = (string[][]) ((string[][]) deafultHeaders).Clone();
            string[] apiKey = { API_KEY_HEADER_NAME, key };

            moduleUtil.ArrayPush(ref copiedHeaders, apiKey);
            deafultHeaders = copiedHeaders;
        }
        
        public void SetLanguage(AirlyLanguage language)
        { 
            string actualCode = Enum.GetName(language.GetType(), language);

            if(string.IsNullOrEmpty(actualCode))
                return;

            actualCode = actualCode.ToLower();
            this.LANGUAGE_CODE = actualCode;
        }

        public void SetLanguage(string language) {
            AirlyLanguage instance = AirlyLanguage.en;
            string[] langNames = Enum.GetNames(instance.GetType());

            string[] correctLangs = new string[0];
            foreach (var lang in langNames)
            {
                string correctedLang = lang.TrimEnd(' ').TrimStart(' ').ToLower();
                moduleUtil.ArrayPush(ref correctLangs, correctedLang);
            }

            string correctLanguage = language.TrimStart(' ').TrimEnd(' ').Replace(" ", "").ToLower();
            bool langCheck = correctLangs.Contains(correctLanguage);
            if (!langCheck) throw new AirlyError(string.Format("Provided language \"{0}\" is invalid", language));

            this.LANGUAGE_CODE = correctLanguage;
        }
        // Initializing the deafult language 
        public void SetLanguage() => SetLanguage(AirlyLanguage.en);

        private HttpMethod GetMethod(string Method)
        {
            HttpMethod httpMethod;
            try
            {
                httpMethod = new HttpMethod(Method.Replace(" ", "").ToUpper());
            }
            catch (Exception)
            { httpMethod = HttpMethod.Get; } // Using GET if the method in string is invalid

            return httpMethod;
        }

        private void SetHeaders(ref HttpClient client, string[][] headers)
        {
            if (headers.Length == 0) throw new Exception("The headers length is 0");

            foreach (var header in headers)
            {
                string name = header[0];
                string value = header[1];

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) continue;

                name = moduleUtil.ReplaceDashUpper(name); // Replacing "Content-Type" to "ContentType"

                bool clientCheck = client.DefaultRequestHeaders.Contains(name);
                if (clientCheck) client.DefaultRequestHeaders.Remove(name);

                // Adding the headers to provided client
                if (client != null) client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
            }
            foreach (var item in client.DefaultRequestHeaders)
            {
                Debug.WriteLine($"{item.Key}     {moduleUtil.GetFirstEnumarable(item.Value)}");
            }
        }
    }
}