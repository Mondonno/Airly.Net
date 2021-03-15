// It is not unit tests but this is my codes trash xD
// If you are looking for tests just look for the tests dir in the Project overlook
// And also you can not see this folder and file because the folder is in git ignore xD (so idk why I'm writing this lol)

using System;
using System.Collections.Generic;
using AirlyAPI;
using AirlyAPI.Handling;
using AirlyAPI.Rest;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using AirlyAPI.Utilities;
using System.Collections;
using AirlyAPI.Handling.Errors;


// The namespace for AirlyAPI unit tests
namespace AirlyAPI
{
    /*
     // Simple get wrapper (because only GET requests Airly API accepts)
        public async Task<T> Api<T>(string end, dynamic query) where T : class => await Request<T>(end, null, query, true);
        public async Task<T> Api<T>(string end, dynamic query, bool versioned) where T : class => await Request<T>(end, null, query, versioned);

     */
    /*
     public class RestApiClientBase 
    {
        public RESTManager RestManager { get; set; }
        public AirlyConfiguration Configuration { get; set; }

        protected RestApiClientBase(RESTManager rest)
        {
            RestManager = rest ?? throw new ArgumentException("rest");
            Configuration = rest.Airly.Configuration;
        }

        protected async Task<T> Api<T>(string end, dynamic query) where T : class => await RestManager.Api<T>(end, query);
        protected string ResolveIndexType(IndexQueryType type) => type == IndexQueryType.AirlyCAQI ? "AIRLY_CAQI" : (type == IndexQueryType.CAQI ? "CAQI" : (type == IndexQueryType.PJP ? "PJP" : "AIRLY_CAQI"));
    }
     */

    /*
     public class RestApiClient : RestApiClientBase
    {
        public RestApiClient(RESTManager rest) : base(rest) { }

        // Installations
        public async Task<Installation> GetInstallationByIdAsync(int id) => await Api<Installation>($"installations/{id}", new { });
        public async Task<List<Installation>> GetInstallationsNearestAsync(double lat, double lng, double maxDistance = 3, int maxResults = 1)
            => await Api<List<Installation>>("installations/nearest", new
            {
                lat,
                lng,
                maxDistanceKM = maxDistance,
                maxResults
            });

        // Measurments
        public async Task<Measurement> GetMeasurmentByInstallationAsync(int id, bool includeWind = false,
            IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/installation", new
            {
                installationId = id,
                includeWind,
                indexType = ResolveIndexType(type)
            });
        public async Task<Measurement> GetMeasurmentByPointAsync(double lat, double lng,
            IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/point", new
            {
                lat,
                lng,
                indexType = ResolveIndexType(type)
            });
        public async Task<Measurement> GetMeasurmentNearestAsync(double lat, double lng, double maxDistance = 3,
            IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/nearest", new
            {
                lat,
                lng,
                maxDistanceKM = maxDistance,
                indexType = ResolveIndexType(type)
            });
        public async Task<Measurement> GetMeasurmentByLocationId(int locationId, bool includeWind = false,
            IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/location", new
            {
                locationId,
                includeWind,
                type = ResolveIndexType(type)
            });


        // Meta
        public async Task<List<IndexType>> GetMetaIndexesAsync() => await Api<List<IndexType>>("meta/indexes", new { });
        public async Task<List<MeasurementType>> GetMetaMeasurmentsAsync() => await Api<List<MeasurementType>>("meta/measurements", new { });

        //        working on the OpenAPI endpoint
        // >> https://airapi.airly.eu/docs/v{versionCode}
        public Task<object> GetAirlyOpenDocsAsync() => null;
    }
     */




    //protected async Task<T> Api<T>(string end, dynamic query) where T : class => await Rest.Api<T>(end, query);
    //AirlyResponse constructedResponse = new AirlyResponse(ConvertJsonString(res), headers, res.RawJson, !string.IsNullOrEmpty(rawDate) ? DateTime.Parse(rawDate) : DateTime.Now);
    /*
     public class AirlyResponse
    {
        public AirlyResponse(dynamic JSON, HttpResponseHeaders headers, string rawJSON, DateTime timestamp)
        {
            this.Json = JSON;
            this.RawJson = rawJSON;
            this.Headers = headers;
            this.Timestamp = timestamp;
        }

        public AirlyResponse(AirlyResponse response, DateTime timestamp)
        {
            this.Json = response.Json;
            this.RawJson = response.RawJson;
            this.Headers = response.Headers;
            this.Timestamp = timestamp;
        }

        public JToken Json { get; }
        public string RawJson { get; }

        public DateTime Timestamp { get; }
        public HttpResponseHeaders Headers { get; }

        public HttpResponseMessage Response { get; set; }
    }
     */
    /*
     using System;
using System.Threading.Tasks;

using AirlyAPI.Handling;
using AirlyAPI.Utilities;
using AirlyAPI.Rest.Typings;

namespace AirlyAPI.Rest
{
    public class RESTManager : BasicRoutes, IDisposable
    {
        private string ApiKey { get; set; }
        private RequestQueueHandler Handlers { get; set; }

        public Airly Airly { get; set; }
        public AirlyLanguage Lang { get; set; } = AirlyLanguage.en;

        public RESTManager(Airly airly, string apiKey)
        {
            this.ApiKey = apiKey;
            this.Airly = airly;
            this.Handlers = new RequestQueueHandler(this);
        }

        public RESTManager(Airly airly) : this(airly, airly.ApiKey) { }

        public string Auth
        {
            get
            {
                if (string.IsNullOrEmpty(ApiKey)) throw new InvalidApiKeyError("Provided api key is empty");
                return this.ApiKey;
            }
        }

        public bool RateLimited
        {
            get
            {
                if (this.Handlers.Queuers() >= 1) return this.Handlers.RateLimited();
                else return false;
            }
        }

        public string Endpoint { get => this.Airly.Configuration.ApiDomain; }
        public string Cdn { get => this.Airly.Configuration.Cdn; }

        private void SetAirlyPreferedLanguage(Airly air) => this.Lang = air.Language;

        public Task<AirlyResponse> Request(string end, string method, RequestOptions options = null)
        {
            if (options == null) options = new RequestOptions();

            if(this.ApiKey != null) options.Auth = true;

            string route = Utils.GetRoute(end);
            DeafultRestRequest Request = new DeafultRestRequest(this, end, method, options);

            Request.SetKey(this.ApiKey);
            Request.SetLanguage(this.Lang);

            return Handlers.Queue(route, Request);
        }
        public async Task<T> RequestAndParseType<T>(string end, string method, dynamic query, bool versioned)
        {
            Utils util = new Utils();
            RequestOptions options = new RequestOptions()
            {
                Query = util.ParseQuery(query),
                Versioned = versioned
            };

            var httpResponse = await Request(end, method ?? "GET", options);
            return new RestResponseParser<T>(httpResponse.rawJSON).Deserializated;
        }

        // Simple get wrapper (because only GET requests Airly API accepts)
        public async Task<T> Api<T>(string end, dynamic query) where T : class => await RequestAndParseType<T>(end, "GET", query, true);
        public async Task<T> Api<T>(string end, dynamic query, bool versioned) where T : class => await RequestAndParseType<T>(end, "GET", query, versioned);

        public void Dispose() => Handlers.Dispose();
    }

    public interface IBaseRouter { }

    public class BasicRoutes : IBaseRouter
    {
        public BasicRoutes(){}

        public override string ToString() => base.ToString();
    }
}

     */

    /*
     public class Handler
    {
        public void MakeRateLimitError(string limits, string all, string customMessage) {
            string errorMessage = $"Your api key get ratelimited for this day/minut/secound by Airly API\n{customMessage ?? ""}\n{all}/{limits}";
            AirlyError error = new AirlyError(errorMessage);
            error.Data.Add("Ratelimited", true);

            throw error;
        }
        public void MakeRateLimitError(RawRestResponse res, Utils utils, string customMessage = null)
        {
            var headers = res.HttpResponse.Headers;

            string limits = utils.GetHeader(headers, "X-RateLimit-Limit-day");
            string all = utils.CalculateRateLimit(headers).ToString();

            MakeRateLimitError(limits, all, customMessage);
        }

        public bool IsMalformedResponse(string JsonString)
        {
            try
            { HandleMalformed(JsonString.ToString()); }
            catch (Exception)
            { return true; } // Malformed

            return false; // Normal response, non malformed
        }

        // Handling the malformed request and throwing a new error
        private void HandleMalformed(string rawJSON)
        {
            try
            { JsonParser.ParseJson(rawJSON); }
            catch (Exception ex)
            { throw new HttpError($"[AIRLY] POSSIBLE MALFORMED RESPONSE\n{ex.Message}"); }
        }

        public void HandleError(int code, RawRestResponse response)
        {
            var utils = new Utils();

            var headers = response.HttpResponse.Headers;
            string rawJSON = response.RawJson;

            if (code > 0 && code <= 200) return; // all ok, returning

            // MOVED_PERMANETLY response code 301
            if(code == 301) {
                HandleMalformed(rawJSON);
                return;
            }
            if (code == 404) {
                HandleMalformed(rawJSON);
                return;
            }; // Passing null on the 404

            int? limit = utils.CalculateRateLimit(headers);
            if (limit == 0) throw new AirlyError($"Get ratelimited by airly api\n{utils.CalculateRateLimit(headers)}");
            if (code > 200 && code <= 300)
            {
                if(code == 301)
                {
                    HandleMalformed(rawJSON);
                    return;
                }
                throw new AirlyError("Unknown invalid request/response");
            }
            if (code >= 400 && code < 500)
            {
                if (code == 401) throw new AirlyError("The provided API Key is not valid");
                if (code == 429)
                {
                    MakeRateLimitError(utils.GetHeader(headers, "X-RateLimit-Limit-day"), $"{utils.CalculateRateLimit(headers)}", "");
                    return;
                }
                HandleMalformed(rawJSON);
                throw new AirlyError($"[AIRLY_INVALID] [{DateTime.Now}] {rawJSON}");
            }
            if (code >= 500 && code < 600) throw new HttpError("[AIRLY] INTERNAL PROBLEM WITH AIRLY API");
        }

        public ErrorModel GetErrorFromJson(JToken json)
        {
            bool errorCheck = json["errorCode"] == null;
            if (errorCheck) return null;

            JToken[] tokens = {
                json["errorCode"],
                json["message"],
                json["details"]
            };
            JObject[] errors = new Utils().ConvertTokens(tokens);

            string code = errors[0].ToString();
            string message = errors[1].ToString();

            JObject details = errors[2];

            // Checking the succesors of the error
            int? succesor;
            if (code == "INSTALLATION_REPLACED" && details["successorId"] != null) succesor = Convert.ToInt32(details["successorId"].ToString());
            else succesor = null;

            var errorModel = new ErrorModel(message, succesor, code);
            return errorModel;
        }
    }
     */
    /*
     
            Location location = new(50.081319, 18.459592);

            Installation installation = (await airly.Installations.Nearest(location))[0];

            Debug.WriteLine(installation.Address.City);
            Debug.WriteLine(installation.Address.Street);
            Debug.WriteLine(installation.Address.Number);

            Measurement measurement = await airly.Measurements.Installation(installation);

            Index index = measurement.Current.Indexes.Find(e => e.Name == "AIRLY_CAQI");
            double? caqi = index.Value;

            Debug.WriteLine(index.Description);

            throw new System.Exception(caqi.ToString());
     */
    /*
     public Airly(Airly airly)
        {
            this.ApiKey = airly.ApiKey;
            this.Language = airly.Language;
            this.Configuration = airly.Configuration;

            Initialize();
        }

        public Airly(Airly airly, AirlyConfiguration configuration)
        {
            this.ApiKey = airly.ApiKey;
            this.Language = airly.Language;
            this.Configuration = configuration;

            Initialize();
        }

        public Airly(string apiKey)
        {
           this.ApiKey = apiKey;
           
           Initialize();
        }

        public Airly(string apiKey, AirlyConfiguration configuration)
        {
            this.ApiKey = apiKey;
            this.Configuration = configuration;

            Initialize();
        }
     */

    /*
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
            { "Accept", "*"/"*" },
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

    if (this.RestOptions.Body != null)
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

public void Refresh()
{
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
     */

    /*
     private object ValidateLang(object lang)
        {
            if (lang == null) return AirlyLanguage.en;

            // Declaring the namespace and the type of the checking object
            string air = string.Format("{0}.{1}", nameof(AirlyAPI), nameof(AirlyLanguage));
            string matchType = lang.GetType().ToString();

            if (matchType == air) return lang;
            else return AirlyLanguage.en;
        }
     */
    /*
     Utils util = new Utils();
            RequestOptions options = new RequestOptions()
            {
                Query = util.ParseQuery(query),
                Versioned = versioned
            };
            var httpResponse = await Request(end, method ?? "GET", options);
            string json;

            if (httpResponse == null) return default;
            else json = httpResponse.rawJSON;

            var jsonParsedResult = JsonParser.DeserializeJson<T>(json);
            if (jsonParsedResult == null) return default;

            return jsonParsedResult;
     */
    /*
     foreach (var mm in measurement.Current.Values)
            {
                Debug.WriteLine($"{mm.Name.ToString()}    {mm.Value.ToString()}");
                // Entered
            }
            Debug.WriteLine("-----------");
            foreach (var index in measurement.Current.Indexes)
            {
                Debug.WriteLine($"{index.Name}    {index.Value}");
            }
            Debug.WriteLine("-----------");
            foreach (var standard in measurement.Current.Standards)
            {
                Debug.WriteLine($"{standard.Name}    {standard.Pollutant}    {standard.Limit}");
            }

            throw new Exception(measurement.Current.TillDateTime.ToString());
     */
    /*
     //// Handling all the RequestQueuers with the specified interaction first endpoints
    //// The small List wrapper with the additional methods
    //public class RequestQueueHandler : List<KeyValuePair<string, RequestQueuer>>
    //{
    //    public int Handlers() => this.Count;
    //    public bool Inactive() => this.Count == 0;

    //    public bool RateLimited()
    //    {
    //        if (this.Count == 0) return false;

    //        List<bool> limits = new List<bool>();
    //        foreach (var queuer in this) limits.Add(queuer.Value.RateLimited);

    //        List<bool> limited = limits.FindAll((e) => e == true);
    //        return limits.Count == limited.Count;
    //    }

    //    public RequestQueuer Get(string route)
    //    {
    //        var value = this.Find((e) => e.Key == route).Value;
    //        return value ?? null;
    //    }

    //    public void Set(string route, RequestQueuer queue)
    //    {
    //        KeyValuePair<string, RequestQueuer> element = new KeyValuePair<string, RequestQueuer>(route, queue);
    //        var checkedValue = Find((obj) => obj.Key == route);
    //        bool checkContains = checkedValue.Value != null;

    //        if (checkContains) {
    //            this.Remove(checkedValue);
    //            this.Add(element);
    //        }
    //        else this.Add(element);
    //    }

    //    // Reseting the queuer handler and destroying all key queuers
    //    public void Reset()
    //    {
    //        foreach (var avaibleQueuer in this)
    //        {
    //            var queuer = avaibleQueuer.Value ?? null;
    //            if (queuer == null || string.IsNullOrEmpty(avaibleQueuer.Key)) continue;

    //            queuer.Zero(); // Detroying
    //            this.Remove(avaibleQueuer); // Removing
    //        }
    //    }
    //}
     */




    /*// tods przepisac request module na HttpRequestMessage (na handlery)

    [Obsolete]
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

     */

    /*
     * //Installation installation = (await airly.Installations.Nearest(rybnik))[0];
            //airly.Measurements.Location(installation.locationId, true).Result.current.fromDateTime.ToString();

            //string json = JsonParser.SerializeJSON<Installation>(installation);

            //Debug.WriteLine(json.ToString());
            //throw new Exception(json.ToString());

            //List<Installation> installations = await airly.Installations.Nearest(rybnik, 100, 10);
            //List<Measurement> measurements = new List<Measurement>();

            //foreach (var installation in installations)
            //{
            //    Measurement measurement = await airly.Measurements.Installation(installation.id);
            //    Debug.WriteLine(installation.id);
            //    measurements.Add(measurement);
            //}

            //List<double> caqis = new List<double>();
            //foreach (var measurement in measurements)
            //{
            //    double caqi = GetMeasurmentAirlyCAQI(measurement);
            //    Debug.WriteLine(caqi);
            //    caqis.Add(caqi);
            //}
            //double avarge = caqis.Average();
            //throw new Exception(avarge.ToString());

            //Measurement value = await airly.Measurements.Nearest(rybnik);
            //Installation installation = (await airly.Installations.Nearest(rybnik))[0];
            //Measurement measurement = await airly.Measurements.Installation(installation.id);

            //Measurement measurement = await airly.Measurements.Nearest(rybnik);

            //foreach (var item in measurement.current.indexes)
            //{
            //    Debug.WriteLine($"{item.name}     {item.value}");
            //}
            //foreach (var item in measurement.current.values)
            //{
            //    Debug.WriteLine($"{item.name}     {item.value}");
            //}

            //double caqi = GetMeasurmentAirlyCAQI(measurement);
            //throw new Exception(
                //installation.location.lat + "   " + installation.location.lng +
                //"  CAQI  " + caqi);
            //Installation installation = await airly.Installations.Info()

            //double caqi = GetMeasurmentAirlyCAQI(value);
            //throw new Exception(value.sponsor.logo.ToString());
            //throw new Exception(caqi.ToString());
    */



    // //AirlyResponse airlyResponse = new AirlyResponse(
    //    convertedJSON,
    //    response.Headers,
    //    responseBody,
    //    DateTime.Now // Only for the raw requests without handlers
    //)
    //{ response = response };

    /*
     * var json = "[ \"admin\", \"editor\", \"contributor\" ]";
var result = JsonConvert.DeserializeObject<Whatever<string>>(json);
foreach (var item in result)
{
Console.WriteLine($"{item}");
}
class Whatever<T> : List<T> {}
     */
    /*
     using System.Text.RegularExpressions;
using System.Net.Http;
using System.Dynamic;
using System.Threading.Tasks;
     */
    /*
     [Obsolete("The characters escaper is not longer in use (no-need-escape)")]
        public string charactersEscaper(string[] characters, string text)
        {
            string textReturn = text;
            string escaper = "\\";
            for (int i = 0; i < characters.Length; i++)
            {
                string character = characters[i];
                textReturn.Replace(character, string.Format("{0}{1}", escaper, character));
            }
            return textReturn;
        }

        // Simple resolving from enum the code of the request
        [Obsolete("unused for the request Module")]
        public int resolveStatusCode(HttpStatusCode code)
        {
            Type enumType = code.GetType();

            string[] codesNames = Enum.GetNames(enumType);
            int[] codes = (int[])Enum.GetValues(enumType);

            string codeName = code.ToString();
            string validCodeName = null;

            foreach (var cd in codesNames)
            {
                if (cd == codeName) validCodeName = cd;
                else continue;
            }

            int validIndex = Array.IndexOf(codesNames, validCodeName);
            var checkedValue = $"{codes[validIndex]}";

            return Convert.ToInt32(checkedValue);
        }
     */
    /*
            var util = new Utils();
            if (options == null) options = new RequestOptions(new string[0][]);

            RequestModule requestManager = new RequestModule(this, end, method.ToUpper(), options);

            requestManager.setKey(apiKey);
            requestManager.SetLanguage(this.Lang);

            var response = await requestManager.MakeRequest();
            string dateHeader = util.getHeader(response.headers, "Date");

            DateTime date = DateTime.Parse(dateHeader ?? DateTime.Now.ToString()); // If the date header is null setting the date for actual date
            return new AirlyResponse(response, date);
    */

    public class unitTests
    {
        //if (i >= (options.query.Length - 1)) queryString = queryString.Substring(0, queryString.Length - 1);
        //             if (string.IsNullOrEmpty(API_KEY) || string.IsNullOrEmpty(API_KEY.Replace(" ", ""))) throw new AirlyError(new HttpError("The provided airly api key is empty"));

        //            if (string.IsNullOrEmpty(apiKey[1])) apiKey[1] = API_KEY.ToString();

        /*
         * 
        public static Type GetTokenType(JTokenType type)
        {
            return type.GetType();
        }

        public static bool checkJsonArray(string json)
        {
            string jsonValidation = json.Trim().ToString();
            bool arrayCheck = jsonValidation.StartsWith("[") && jsonValidation.EndsWith("]");
            return arrayCheck;
        }
        public static bool checkJsonObject(string json)
        {
            string jsonValidation = json.Trim().ToString();
            bool objectCheck = jsonValidation.StartsWith("{") && jsonValidation.EndsWith("}");
            return objectCheck;
        }
        public static bool checkJsonValidation(string json)
        {
            bool isJsonObject = checkJsonObject(json);
            bool isJsonArray = checkJsonArray(json);

            bool isValidJson = isJsonObject && isJsonArray;
            return isValidJson;
        }

        // Simple parsing wrapper
        public static JToken ParseJson(string json)
        {
            Utils utils = new Utils();
            JObject[] finalArrayResult = new JObject[0];
            if (checkJsonArray(json))
            {
                var arr = JArray.Parse(json);
                foreach (JObject obj in arr) utils.ArrayPush(ref finalArrayResult, obj);
                if (finalArrayResult.Length == 0) return new JArray();

                return arr;
            }
            else if (checkJsonObject(json)) return JObject.Parse(json);
            else return new JObject();
        }

        public static T ParseToClassJSON<T>(string json)
        {
            // Some date handlings
            JsonSerializerSettings settings = new JsonSerializerSettings() {
                DateFormatString = "yyyy-MM-ddTH:mm:ss.fffK",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            };

            string rawjson = json.ToString();
            T classment = JsonConvert.DeserializeObject<T>(rawjson, settings);
            return classment;
        }
        public static T ParseToClassJSON<T>(JToken jsonToken) => ParseToClassJSON<T>(jsonToken.ToString());
         */
        public void test()
        {
            //object lang = this.Lang;
            //AirlyLanguage validatedLang = (AirlyLanguage)ValidateLang(lang);

            //// toddo Only for the debug tests (to delete)
            //foreach (var item in client.DefaultRequestHeaders)
            //{
            //    Debug.WriteLine($"{item.Key}     {moduleUtil.getFirstEnumarable(item.Value)}");
            //}


            //setLanguage(language == "en" ? AirlyLanguage.en : (language == "pl" ? AirlyLanguage.pl : AirlyLanguage.en));
            //string virtualHost = $"https://{API_DOMAIN}"; // Declaring the virtual host to get the query from URI
            //
            //Uri param = new Uri(virtualHost);
            //Uri constructedQuery = new Uri(string.Format("{0}{1}{2}", virtualHost, this.endPoint,
            //        (queryString != "" ? string.Format("?{0}",
            //        queryString.Replace("#", "%23").Replace("@", "%40")) // Replacing # and @ to the own URL code (Uri does not support # and @ in query)
            //        : ""))); // no need to parse the Query (eg. % === %25) (the Uri class do this for me)
        }
        //[TestMethod]
        public async void RunTests()
        {
            string apiKey = "121§324323c231xdsadI21OE";
            Airly airly = new Airly(apiKey);
            int id = 1;
            Installation results = null;

            // Recommended to use try catch when u want to use the redirect option
            // (Throwing error if the new Installation ID does not exists)
            try
            {
                 _ = results != null ? results = await airly.Installations.Info(id, true) : null;
            }
            catch (AirlyError ex)
            {
                // Console Line Writing the new succesor ID if the succesor is in the new of new Response
                // (To prevent infinity loops and the inifnity limit usage we throw error if the new succesor id does not exists)
                if(ex.Data["succesor"].ToString() != null)
                    Console.WriteLine($"The new succesor id: {ex.Data["succesor"]}");
            }
            if(results != null)
            {
                // Some actions with results
            }
        }
        public void RunBasicAirlyTest()
        {
            //string apiKey = "123213125sdfggsaete3123";
            //Airly airly = new Airly(apiKey);

            //double lat = 12.035;
            //double lng = 13.01;
            //double maxDistance = 12.334;

            //var measurements = await airly.Measurements.Nearest(lat, lng, maxDistance, IndexQueryType.CAQI);
            //List<DateTime> datetimes = new List<DateTime>();
            //foreach (var item in measurements)
            //{
            //    string rawdate = item.current.fromDateTime.ToString();
            //    DateTime date = DateTime.Parse(rawdate);
            //    datetimes.Add(date);
            //}
        }
        public void RunInstallTests()
        {
            //string apiKey = "123213125sdfggsaete3123";
            //Airly airly = new Airly(apiKey);
            //double lat = 12.035;
            //double lng = 13.01;
            ////double maxDistance = 12.334;
            //var Measurements = await airly.Measurments.Point(lat, lng, IndexQueryType.AirlyCAQI);

        }

        // Running the request tests
        public static void RunRequest()
        {

            //string apiKey = "12233333";

            //var properties = new AirlyProps(new Types());
            //properties.API_KEY = string.Format("{0}", apiKey);

            //var requestResponse = new RequestModule("meta", "", null, properties);

            //Console.WriteLine(requestResponse);
        }
        public static void Run()
        {
            RunRequest(); 
        }
        public static void Main(string[] args)
        { 
            Console.WriteLine("Started Unit Tests");
            Run();
        }
    }
}

//// Checking if the custom headers contains the api key and then setting them to the apiKey header
//            if (deafultHttpHeaders.Contains(API_KEY_HEADER_NAME)) {
//                string firstValue = moduleUtil.getHeader(deafultHttpHeaders, API_KEY_HEADER_NAME);

//_ = apiKey[0] == API_KEY_HEADER_NAME? apiKey[1] = firstValue : apiKey[1] = "";
//            }
//            else

//if (moduleUtil.Exists(customHeaders, API_KEY_HEADER_NAME) > -1)
//            {
//                string key = customHeaders[(moduleUtil.Exists(customHeaders, API_KEY_HEADER_NAME))][1];
//apiKey[1] = key;
//            }