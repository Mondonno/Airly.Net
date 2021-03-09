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

// The namespace for AirlyAPI unit tests
namespace AirlyAPI
{
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
        public async void RunBasicAirlyTest()
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