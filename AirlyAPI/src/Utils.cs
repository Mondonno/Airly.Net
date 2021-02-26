using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;

namespace AirlyAPI
{
    public interface IUtils
    { }
    public class Utils : IUtils
    {

        public static string domain = "airly.eu";

        // The ratelimits headers names
        private string XRemainingName = "X-RateLimit-Remaining-day";
        private string XLimitName = "X-RateLimit-Limit-day";

        // Checking if the ratelimit is reached
        private bool getRateLimitBase(string XRemaining, string XLimit)
        {
            int rateLimitRemaining;
            int rateLimitAll;
            int rateLimit;

            bool rateLimitCheck = false;

            bool headersExists = XRemaining == null || XRemaining == "" || XLimit == null || XLimit == "";

            if (!headersExists)
            {
                rateLimitRemaining = Convert.ToInt32(XRemaining);
                rateLimitAll = Convert.ToInt32(XLimit);

                // Like
                // 100 (ratelimit) - 23 (uses) = 77
                rateLimit = rateLimitAll - rateLimitRemaining;

                // If the ratelimit is reached the rateLimit value is 100 (any avaible request can now be sent after ratelimit reset)
                rateLimitCheck = rateLimit == 100;

                return rateLimitCheck;
            }

            return rateLimitCheck;
        }

        // Getting and calculating the ratelimits for the headers
        public bool getRatelimit(HttpResponseHeaders headers)
        {
            string XRemaining = getHeader(headers, this.XRemainingName);
            string XLimit = getHeader(headers, this.XLimitName);

            object rateLimit = getRateLimitBase(XRemaining, XLimit);
            return (bool)rateLimit;
        }

        // The method to get the ratlimits from the response message
        public bool getRatelimit(AirlyResponse response)
        {
            HttpResponseHeaders headers = response.headers;

            string XRemaining = getHeader(headers, this.XRemainingName);
            string XLimit = getHeader(headers, this.XLimitName);

            object rateLimit = getRateLimitBase(XRemaining, XLimit);
            return (bool)rateLimit;
        }

        // Calculating the ratelimits diffrents
        public int calculateRateLimit(AirlyResponse response)
        {
            var headers = response.headers;

            string XRemaining = getHeader(headers, this.XRemainingName);
            string XLimit = getHeader(headers, this.XLimitName);

            if (XRemaining == null || XLimit == null) return 0;

            int cnv1 = Convert.ToInt32(XRemaining);
            int cnv2 = Convert.ToInt32(XLimit);

            int calculated = cnv2 - (cnv2 - cnv1);
            return calculated;
        }

        private string getHeaderBase(IEnumerable<string> values) => this.getFirstEnumarable(values);

        // For response headers
        // Getting the header first value because the headers can have multiple values (Airly API always return one value headers)
        public string getHeader(HttpResponseHeaders headers, string key) => getHeaderBase(headers.GetValues(key));

        // For request headers
        // 
        public string getHeader(HttpRequestHeaders headers, string key) => getHeaderBase(headers.GetValues(key));

        [Obsolete("The characters escaper is not longer in use (no-need-escape)")]
        public string charactersEscaper(string[] characters, string text) {
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
            int[] codes = (int[]) Enum.GetValues(enumType);

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

        public string replaceDashUpper(string str)
        {
            string finalString = "";
            string[] strs = str.Split('-');
            foreach (string nm in strs)
            {
                finalString += string.Format("{0}{1}", nm[0].ToString().ToUpper(), nm.Remove(0, 1));
            }
            return finalString;
        }

        public class NormalizedProperty
        {
            public NormalizedProperty(string name, object value)
            {
                this.name = name;
                this.value = value;
            }

            public string name { get; set; }
            public object value { get; set; }
        }

        // Klasy anonimowe sa odczytywane jako normalne klasy bez dziedziczenia oraz bez konstruktora. system wartości PropertyInfo jest identyczny
        public List<NormalizedProperty> GetClassProperties<T>(T classObject)
        {
            Type classType = classObject.GetType();
            PropertyInfo[] props = classType.GetProperties();
            List<NormalizedProperty> normalizedProperties = new List<NormalizedProperty>();

            foreach (var prop in props)
            {
                string name = prop.Name;
                MethodInfo method = prop.GetGetMethod();

                if (!prop.CanRead) continue;
                if (method == null) continue;

                Type propType = prop.PropertyType; // cs-unused
                object desiredValue = method.Invoke(classObject, null);

                object[] values = new object[2] { name, desiredValue };

                NormalizedProperty normalized = new NormalizedProperty((string)values[0], values[1]);
                normalizedProperties.Add(normalized);
            }

            return normalizedProperties;
        }

        public string[][] ParseQuery(dynamic query)
        {
            List<NormalizedProperty> properties = GetClassProperties(query);

            string[][] convertedQuery = { };
            foreach (var p in properties)
            {
                string name = p.name;
                string value = string.Format("{0}", p.value); // Converting the object value to string (without the explict type)

                _ = value == null ? value = "" : null;

                string[] constructedArray = { name, value };
                this.ArrayPush(ref convertedQuery, constructedArray);
            }

            return convertedQuery;
        }

        // Validating the 
        public bool validateKey(string key)
        {

            return false;
        }

        public T getFirstEnumarable<T>(IEnumerable<T> enumarable) => enumarable.First((e) => true);

        // Simple coping the one dictonary to another without the overwriting
        public void CopyDictonaryValues(ref IDictionary<object, object> target, IDictionary<object, object> toCopyInto)
        {
            foreach (var pair in toCopyInto)
                target.Add(pair.Key, pair.Value);
        }

        public JObject[] convertTokens(JToken[] tokens)
        {
            JObject[] jObjects = new JObject[0];
            foreach (var token in tokens)
            {
                ArrayPush(ref jObjects, (JObject)token);
            }
            return jObjects;
        }

        public static Type GetTokenType(JTokenType type)
        {
            return type.GetType();
        }

        // Simple parsing wrapper
        public static JObject ParseJson(string json)
        {
            JObject convertedJSON = JObject.Parse(json);
                //JsonConvert.DeserializeObject(json);
            return convertedJSON;
        }
        public static T ParseToClassJSON<T>(JObject json)
        {
            // Some date handlings
            JsonSerializerSettings settings = new JsonSerializerSettings() {
                DateFormatString = "yyyy-MM-ddTH:mm:ss.fffK",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var rawjson = JsonConvert.SerializeObject(json);
            T classment = JsonConvert.DeserializeObject<T>(rawjson, settings);
            return classment;
        }

        // <===========>
        //  Array utils
        // <===========>
        public void ArrayPush<T>(ref T[] table, object value)
        {
            Array.Resize(ref table, table.Length + 1); // Resizing the array for the cloned length (+-) (+1-1+1)
            table.SetValue(value, table.Length - 1);
        }

        // Checking if the element exists in [][] Array in Array array
        public int Exists(string[][] table, string value)
        {
            int index = -1;
            foreach (var tab in table)
            {
                bool end = false;
                for (int i = 0; i < tab.Length; i++)
                    if (tab[i] == value) { index = Array.IndexOf(table, tab); end = true;  break; };

                if (end == true) break;
            }
            return index;
        }

        public T[] removeArrayValue<T> (T[] table, int index)
        {
            // Clone from start index of the element and then pause
            // Then clone from the index + 1 and add this to the final array

            T[] newArray = (T[]) table.Clone();

            int lastIndex = index + 1;
            int calced = (table.Length - 1) - lastIndex;

            Array.ConstrainedCopy(table, 0, newArray, 0, index);
            Array.ConstrainedCopy(table, lastIndex, newArray, index, calced);

            return newArray;
        }

        public string[] removeStringEmtpyValues(string[] table)
        {
            string[] newTabel = new string[table.Length];
            foreach (var item in table)
            {
                var ii = item;
                int index = Array.IndexOf(newTabel, ii);
                if (!string.IsNullOrEmpty(ii)) {
                    newTabel = removeArrayValue(newTabel, index);
                }
            }
            return newTabel;
        }

        public string joinArray<T> (ref T[] table, string character)
        {
            string compiledString = "";
            foreach (var item in table)
            {
                compiledString += string.Format("{0}{1}", item, character);
            }
            return compiledString;
        }

        public T[] assignArray<T>(T[] source, T[] target) {
            T[] newArray = (T[]) target.Clone();

            int calculatedLength = target.Length + source.Length;
            int oldLength = target.Length;

            Array.Resize(ref newArray, calculatedLength);
            Array.ConstrainedCopy(source, 0, newArray, oldLength, source.Length);

            return newArray;
        }
    }
}