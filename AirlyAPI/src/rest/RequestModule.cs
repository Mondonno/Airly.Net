using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;

using AirlyAPI.Utilities;

namespace AirlyAPI.Rest
{
    [DnsPermission(System.Security.Permissions.SecurityAction.Assert)]
    [WebPermission(System.Security.Permissions.SecurityAction.Assert)]
    /// <summary>Raw request module for Airly API Wrapper *(Do not use this in yours project, just check API manager)</summary>
    public class RequestModule
    {
        private int Version { get; set; } = 2;
        private string ApiDomain { get; set; }

        private string LanguageCode { get; set; } = "en"; // Deafult language code is the english

        private Utils ModuleUtil { get; set; } = new Utils();
        private AirlyConfiguration Configuration { get; set; } // The airly http configuration

        private RequestOptions Options { get; set; }

        private string ApiUrl { get; set; }
        private string AuthorizationHeaderName { get; set; } = "apikey";

        public object DeafultHeaders { get; set; }
        public string method, endPoint, path;

        /// <summary>Raw request module for Airly API Wrapper <i>(Do not use this in yours project, just check API manager)</i></summary>
        public RequestModule(RESTManager rest, string endPoint, string method, RequestOptions options)
        {
            AirlyConfiguration rawConfig = rest.Airly.Configuration; PatchConfiguration(ref rawConfig);

            this.Configuration = rawConfig;
            this.Options = options;
            this.method = method;
            this.endPoint = endPoint;

            string[] agent = { "User-Agent", this.Configuration.Agent };
            string[] connection = { "Connection", "keep-alive" };
            string[] cacheControl = { "Cache-Control", "no-cache" };
            string[] encoding = { "Accept-Encoding", "gzip" };

            string jsonResponses = "application/json";
            string[] accept = { "Accept", jsonResponses };
            string[] contentType = { "Content-Type", jsonResponses };

            DeafultHeaders = new string[][] {
                agent, connection,
                cacheControl, encoding,
                accept, contentType
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
                }

                queryString = queryString.EndsWith("&") ? queryString.Remove(queryString.Length - 1, 1) : queryString;
                finalQuery = ModuleUtil.FormatQuery(queryString);
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
            if(customHeaders == null) customHeaders = new string[0][];

            // Initializing the request headers used in initializing the HttpClient
            string[][] requestHeaders;

            // The request response timeout
            double restTimeout = this.Configuration.RestRequestTimeout;
            double timeout = restTimeout == 0 ? 60000 : restTimeout;

            string[] apiKey = { AuthorizationHeaderName, "key" };
            string[] contentType = { "Content-Type", "application/json" };
            string[] language = { "Accept-Language", LanguageCode ?? "en" };
            // Accept Languages:
            // * pl
            // * en

            ArrayUtil.ArrayPush(ref customHeaders, language);
            if (!string.IsNullOrEmpty(body)) ArrayUtil.ArrayPush(ref customHeaders, contentType);

            // when the custom headers exists we assigning the on array to another
            string[][] copiedDeafultHeaders = (string[][]) ((string[][]) DeafultHeaders).Clone();
            string[] customKey = Array.Find(customHeaders, (key) => key[0] == AuthorizationHeaderName);

            if (customKey != null && !string.IsNullOrEmpty(customKey[1])) apiKey[1] = string.Format("{0}", customKey[1]);
            if ((customHeaders.Length - 1) >= 0) {
                customHeaders = ArrayUtil.AssignArray(copiedDeafultHeaders, customHeaders);
            }
            else customHeaders = copiedDeafultHeaders;
            requestHeaders = customHeaders;

            string RequestWebUrl = ApiUrl + path;
            
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
            Debug.WriteLine(responseBody.ToString());

            RequestClient.Dispose();

            JToken convertedJSON = JsonParser.ParseJson(responseBody);

            AirlyResponse airlyResponse = new AirlyResponse(
                convertedJSON,
                response.Headers,
                responseBody,
                DateTime.Now // Only for the raw requests without handlers
            )
            { response = response };
            response.Dispose();

            return airlyResponse;
        }

        public void SetKey(string key)
        {
            if (!this.Options.auth) return;

            string[][] copiedHeaders = (string[][]) ((string[][]) DeafultHeaders).Clone();
            string[] apiKey = { AuthorizationHeaderName, key };

            ArrayUtil.ArrayPush(ref copiedHeaders, apiKey);
            DeafultHeaders = copiedHeaders;
        }
        public void SetKey(RESTManager rest) { if (this.Options.auth != false) SetKey(rest.Auth); }
        
        public void SetLanguage(AirlyLanguage language)
        { 
            string actualCode = Enum.GetName(language.GetType(), language);

            if(string.IsNullOrEmpty(actualCode))
                return;

            actualCode = actualCode.ToLower();
            this.LanguageCode = actualCode;
        }

        public void SetLanguage(string language) {
            AirlyLanguage instance = AirlyLanguage.en;
            string[] langNames = Enum.GetNames(instance.GetType());

            string[] correctLangs = new string[0];
            foreach (var lang in langNames)
            {
                string correctedLang = lang.TrimEnd(' ').TrimStart(' ').ToLower();
                ArrayUtil.ArrayPush(ref correctLangs, correctedLang);
            }

            string correctLanguage = language.TrimStart(' ').TrimEnd(' ').Replace(" ", "").ToLower();
            bool langCheck = correctLangs.Contains(correctLanguage);
            if (!langCheck) throw new AirlyError(string.Format("Provided language \"{0}\" is invalid", language));

            this.LanguageCode = correctLanguage;
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

        private void PatchConfiguration(ref AirlyConfiguration config)
        {
            // string.Format("https://{0}/v{1}/", ApiDomain, Version)
            AirlyConfiguration configuration = config;

            this.Version = configuration.Version;
            this.ApiDomain = configuration.ApiDomain;
            this.ApiUrl = string.Format("{0}://{1}/v{2}/", configuration.Protocol, ApiDomain, Version);

            config = configuration;
        }

        public void SetHeader(ref HttpClient client, string[] header)
        {
            string name = ModuleUtil.ReplaceDashUpper(header[0]);
            string value = header[1];

            bool check = client.DefaultRequestHeaders.Contains(name);
            if (check) client.DefaultRequestHeaders.Remove(name);

            client.DefaultRequestHeaders.Add(name, value);
        }

        private void SetHeaders(ref HttpClient client, string[][] headers)
        {
            if (headers.Length == 0) throw new Exception("The headers length is 0");

            foreach (var header in headers)
            {
                string name = header[0];
                string value = header[1];

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) continue;

                name = ModuleUtil.ReplaceDashUpper(name); // Replacing "Content-Type" to "ContentType"

                bool clientCheck = client.DefaultRequestHeaders.Contains(name);
                if (clientCheck) client.DefaultRequestHeaders.Remove(name);

                // Adding the headers to provided client
                if (client != null) client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
            }
            foreach (var item in client.DefaultRequestHeaders)
            {
                Debug.WriteLine($"{item.Key}     {ModuleUtil.GetFirstEnumarable(item.Value)}");
            }
        }
    }
}