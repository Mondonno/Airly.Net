using System;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace AirlyAPI
{
    [DnsPermission(System.Security.Permissions.SecurityAction.Assert)]
    [WebPermission(System.Security.Permissions.SecurityAction.Assert)]
    /// <summary>Raw request module for Airly API Wrapper *(Do not use this in yours project, just check API manager)</summary>
    public class RequestModule
    {
        private static int VERSION = 2;
        private static string API_DOMAIN = $"airapi.{Utils.domain}";

        private string LANGUAGE_CODE = "en"; // Deafult language code is the english
        private protected string API_KEY = null;
       
        public string API_URL = string.Format("https://{0}/v{1}/", VERSION, API_DOMAIN);
        public string API_KEY_HEADER_NAME = "apikey";

        public AirlyProps airlyProperties { get; set; }
        public HttpRequestHeaders deafultHttpHeaders { get; set; }
        private Utils moduleUtil = new Utils();

        public object options { get; set; }
        public string apiToken { get; set; }

        public object deafultHeaders { get; set; }

        public string method = "get", Agent, endPoint = "";
        public string path { get; set; }

        /// <summary>Raw request module for Airly API Wrapper *(Do not use this in yours project, just check API manager)</summary>
        public RequestModule(string endPoint, string method, RequestOptions options, AirlyProps airlyProperties)
        {
            this.options = options;
            this.airlyProperties = airlyProperties;

            this.method = method;
            this.endPoint = endPoint;

            // Decalaring all deafult headers
            string[] agent = { "User-Agent", "Airly-C#-Wrapper" };
            string[] connection = { "Connection", "keep-alive" };
            string[] cacheControl = { "Cache-Control", "no-cache" };
            string[] encoding = { "Accept-Encoding", "gzip" }; // Added encoding
            string[] accept = { "Accept", "application/json" };
            
            string[][] headers = new string[][] {
                agent,
                connection,
                cacheControl,
                encoding,
                accept
            };

            deafultHeaders = headers;

            string queryString = "";
            string virtualHost = $"https://{API_DOMAIN}"; // Declaring the virtual host to get the query from URI
            Uri param = new Uri(virtualHost);
            if (options.query != null && options.query.Length != 0)
            {
                options.query = new Utils().ParseQuery(options.query);
                for (int i = 0; i < options.query.Length; i++)
                {
                    string key = options.query[i][0];
                    string value = options.query[i][1];

                    queryString += string.Format("{0}={1}&", key, value);
                }
               
                param = new Uri(string.Format("{0}{1}{2}", virtualHost, this.endPoint,
                    (queryString != "" ? string.Format("?{0}",
                    queryString.Replace("#", "%23").Replace("@", "%40")) // Replacing # and @ to the own URL code (Uri does not support # and @ in query)
                    : ""))); // no need to parse the Query (eg. % === %25) (the Uri class do this for me)

            }
            // Eg. Retruns from https://example.com/?test=Absurdal_test --> /?test=Absurdal_test
            this.path = $"{param.AbsolutePath}{param.Query}";

            apiToken = airlyProperties.API_KEY; // Setting the api key to the props storage value
        }

        private HttpMethod GetMethod(string Method) {
            HttpMethod hmethod;
            string method = Method.ToUpper();

            // Providing support only for delete, post and get (Airly API does not have more requiments)
            if (method == "GET") hmethod = HttpMethod.Get;
            else if (method == "POST") hmethod = HttpMethod.Post;
            else if (method == "DELETE") hmethod = HttpMethod.Delete;
            else hmethod = HttpMethod.Get;

            return hmethod;
        }

        public void setKey(string key)
        {
            string[][] hdd = (string[][])((string[][])deafultHeaders).Clone();
            string[] apiKey = { API_KEY_HEADER_NAME, key };
            moduleUtil.ArrayPush(ref hdd, apiKey);
            deafultHeaders = hdd;
        }

        public void setLanguage(AirlyLanguage language = AirlyLanguage.en)
        {
            var en = AirlyLanguage.en;
            var pl = AirlyLanguage.pl;

            if (language == en) this.LANGUAGE_CODE = "pl";
            else if (language == pl) this.LANGUAGE_CODE = "en";
            else this.LANGUAGE_CODE = "en";
        }

        public void setLanguage(string language) => setLanguage(language == "en" ? AirlyLanguage.en : (language == "pl" ? AirlyLanguage.pl : AirlyLanguage.en));

        /// <summary>
        /// Adding the custom headers to deafult headers
        /// </summary>
        /// <param name="headers"></param>
        private HttpClient SetHeaders(string[][] headers, HttpClient client = null)
        {
            if (headers.Length == 0) {
                throw new Exception("The headers length is 0");
            }

            foreach (var header in headers) {
                string name = header[0] != "" ? header[0] : "Unknown";
                string value = header[1] != "" ? header[1] : "Unknown";

                // Removing if already exists
                if (this.deafultHttpHeaders.Contains(name)) this.deafultHttpHeaders.Remove(name);
                if (client.DefaultRequestHeaders.Contains(name)) client.DefaultRequestHeaders.Remove(name);

                // Adding the header to local headers
                this.deafultHttpHeaders.Add(name, value);

                // Adding the headers to provided client
                if (client != null) {
                    client.DefaultRequestHeaders.Add(name, value);
                }
            }
            return client;
        }

        /// <summary>Makeing a request to the Airly API.
        /// Settings:                        
        /// 
        ///    - endpoint(optional) -- Custom Endpoint(Change if required custom request on the same options)
        ///    - body(optional) -- Body of the request(Used on post). Throw please to your type(eg.FormData)</summary>
        ///<param name="endpoint">Endpoint (optional param)</param>
        ///
        public async Task<AirlyResponse> MakeRequest(string[][] customHeaders = null, string body = null)
        {
            if (API_KEY.Replace(" ", "") == "" || API_KEY == null) throw new AirlyError(new HttpError("The provided airly api key is empty"));

            // Initializing the request headers used in initializing the HttpClient
            string[][] requestHeaders;

            // The request response timeout
            int timeout = 60000;

            string[] apiKey = { API_KEY_HEADER_NAME, "unknown-api-key" };
            string[] contentType = { "Content-Type", "application/json" };
            string[] language = { "Accept-Language", LANGUAGE_CODE ?? "en" };
            // Accept Languages:
            // * pl
            // * en

            moduleUtil.ArrayPush(ref customHeaders, language);
            if (body != null || body != "") moduleUtil.ArrayPush(ref customHeaders, contentType);

            // Checking if the custom headers contains the api key and then setting them to the apiKey header
            if (deafultHttpHeaders.Contains(API_KEY_HEADER_NAME)) {
                string firstValue = moduleUtil.getHeader(deafultHttpHeaders, API_KEY_HEADER_NAME);

                _ = apiKey[0] == API_KEY_HEADER_NAME ? apiKey[1] = firstValue : apiKey[1] = "";
            }
            else if (moduleUtil.Exists(customHeaders, API_KEY_HEADER_NAME) > -1)
            {
                string key = customHeaders[(moduleUtil.Exists(customHeaders, API_KEY_HEADER_NAME))][1];
                apiKey[1] = key;
            }

            if (apiKey[1] != null || apiKey[1] != "") apiKey[1] = apiToken.ToString();

            // when the custom headers exists we assigning the on array to another
            string[][] copiedDeafultHeaders = (string[][])((string[][])deafultHeaders).Clone();
            if ((customHeaders.Length - 1) >= 0) {
                customHeaders = moduleUtil.assignStringArray(ref copiedDeafultHeaders, ref customHeaders);
            }
            else customHeaders = copiedDeafultHeaders;

            string REQ_RL = string.Format("{0}{1}", API_URL, path);

            Uri RequestUri = new Uri(REQ_RL);

            AuthenticationHeaderValue auth_key = new AuthenticationHeaderValue(apiKey[0], apiKey[1]);

            HttpRequestMessage RequestParams = new HttpRequestMessage();
            HttpClient RequestClient = new HttpClient();

            requestHeaders = customHeaders;
            RequestClient = SetHeaders(requestHeaders, RequestClient);

            RequestParams.Method = GetMethod(this.method);
            RequestClient = new HttpClient();

            RequestClient.CancelPendingRequests();

            RequestClient.BaseAddress = RequestUri;
            RequestClient.DefaultRequestHeaders.Authorization = auth_key;

            RequestClient.Timeout = TimeSpan.FromMilliseconds(timeout);

            HttpResponseMessage response = await RequestClient.GetAsync(RequestUri);
            string responseBody = await response.Content.ReadAsStringAsync();

            RequestClient.Dispose();

            JObject convertedJSON = Utils.ParseJson(responseBody);

            AirlyResponse airlyResponse = new AirlyResponse(
                convertedJSON,
                response.Headers,
                responseBody,
                DateTime.Now // Only for the raw requests
            );

            // Working on rate limiter
            //RateLimiter.makeLimit(airlyResponse);
            //RateLimitMessage rateLimit = RateLimiter.limitMessage ?? null;
            //object retrunValue;
            //_ = rateLimit == null ? retrunValue = airlyResponse : retrunValue = rateLimit;

            return airlyResponse;
        }
    }
}