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

        private string getHeaderBase(IEnumerable<string> values)
        {
            string firstValue = "";
            int index = 0;

            foreach (var value in values)
            {
                if (index == 0) firstValue = value;
                index++;
            }

            return firstValue;
        }

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

        // Parsing query to string[][]
        public string[][] ParseQuery(IEnumerable<dynamic> query)
        {
            string[][] queryTable = new string[1][];
            foreach (dynamic q in query)
            {
                string name = nameof(q);
                string[] table = { name, $"{q}" };
                ArrayPush(ref queryTable, table);
            }
            return queryTable;
            
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
                if (ii == null) ii = "";
                if (ii != "") {
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

        public T[] assignStringArray<T>(ref T[] source, ref T[] target) {
            T[] newArray = (T[]) target.Clone();

            int calculatedLength = target.Length + source.Length;
            int oldLength = target.Length;

            Array.Resize(ref target, calculatedLength);
            Array.ConstrainedCopy(source, 0, newArray, oldLength, source.Length);

            return newArray;
        }
    }
}