using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Reflection;
using System.Globalization;

using AirlyNet.Rest.Typings;
using AirlyNet.Handling.Exceptions;
using AirlyNet.Handling;

namespace AirlyNet.Utilities
{
    public static class Utils
    {
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

        // Formmatting web query
        public static string FormatQuery(string query)
        {
            if (query.StartsWith("?")) query = query.Remove(0, 1);

            string virtualHost = "http://127.0.0.1";
            Uri constructedQuery = new Uri(string.Format("{0}{1}{2}", virtualHost, "/",
                    (!string.IsNullOrEmpty(query) ? string.Format("?{0}",
                    query.Replace("#", "%23").Replace("@", "%40")) // Replacing # and @ to the own URL code (Uri does not support # and @ in query)
                    : ""))); // no need to parse the Query (eg. % === %25) (the Uri class do this for me)

            string Query = constructedQuery.Query;
            return Query;
        }

        public static List<NormalizedProperty> GetClassProperties<T>(T classObject)
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

                object desiredValue = method.Invoke(classObject, null);
                object[] values = new object[2] { name, desiredValue };

                NormalizedProperty normalized = new NormalizedProperty((string)values[0], values[1]);
                normalizedProperties.Add(normalized);
            }

            return normalizedProperties;
        }

        public static bool IsDouble(object obj)
        {
            double? result;
            try
            {
                result = (double)obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            if (result != null) return true;
            else return false;
        }

        public static List<List<string>> ParseQuery(dynamic query)
        {
            NumberFormatInfo numberInfo = new NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            if (query == null)
                return default;

            List<NormalizedProperty> properties = GetClassProperties(query);
            List<List<string>> convertedQuery = new List<List<string>>();

            foreach (var p in properties)
            {
                string name = p.name;
                string value; // Converting the object value to string (without the explict type)

                if (IsDouble(p.value)) value = ((double)p.value).ToString(numberInfo);
                else value = p.value.ToString();

                List<string> constructedArray = new List<string>() { name, value };
                convertedQuery.Add(constructedArray);
            }

            return convertedQuery;
        }

        public static T GetFirstEnumarable<T>(IEnumerable<T> enumarable) => enumarable.First((e) => true);

        public static string GetVersion(int version, bool slash) => $"{(slash ? "/" : string.Empty)}v{version}{(slash ? "/" : string.Empty)}";

        public static void ValidateKey(string key)
        {
            // Simple airly key validation (eg. key of whitespaces)
            string toValidate = key;
            string validatedKey = toValidate.Replace(" ", "").Trim().Normalize();

            if (string.IsNullOrEmpty(toValidate) || string.IsNullOrEmpty(validatedKey) || validatedKey != toValidate)
                throw new InvalidApiKeyException();
            else return;
        }

        public static string GetRoute(string url)
        {
            if (url == null) return null;
            string[] routes = url.Split('/');

            // || (routes.Length == 1 && routes[0] == url)
            if (routes.Length == 0) return url;
            else return routes[0].ToString();
        }
    }

    public static class RatelimitsUtil
    {
        private static bool GetRateLimitBase(string XRemaining, string XLimit)
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
            }

            return rateLimitCheck;
        }

        public static bool GetRatelimit(HttpResponseHeaders headers)
        {
            string XRemaining = RestUtil.GetHeader(headers, RateLimitInfo.XRemainingName);
            string XLimit = RestUtil.GetHeader(headers, RateLimitInfo.XLimitName);

            object rateLimit = GetRateLimitBase(XRemaining, XLimit);
            return (bool)rateLimit;
        }

        public static bool GetRatelimit(RestResponse response)
        {
            HttpResponseHeaders headers = response.ResponseHeaders;

            string XRemaining = RestUtil.GetHeader(headers, RateLimitInfo.XRemainingName);
            string XLimit = RestUtil.GetHeader(headers, RateLimitInfo.XLimitName);

            object rateLimit = GetRateLimitBase(XRemaining, XLimit);
            return (bool)rateLimit;
        }

        public static int? CalculateRateLimit(int? XRemaining, int? XLimit)
        {
            if (XRemaining == null || XLimit == null) return null;

            int? calculated = XLimit - (XLimit - XRemaining);
            return calculated;
        }

        public static int? CalculateRateLimit(RestResponse res) => CalculateRateLimit(res.ResponseHeaders);

        public static int? CalculateRateLimit(HttpResponseHeaders responseHeaders)
        {
            var headers = responseHeaders;

            string XRemaining = RestUtil.GetHeader(headers, RateLimitInfo.XRemainingName);
            string XLimit = RestUtil.GetHeader(headers, RateLimitInfo.XLimitName);

            if (XRemaining == null || XLimit == null) return null;

            int cnv1 = Convert.ToInt32(XRemaining);
            int cnv2 = Convert.ToInt32(XLimit);

            return CalculateRateLimit(cnv1, cnv2);
        }
    }

    public static class RestUtil
    {
        // For response headers
        // Getting the header first value because the headers can have multiple values (Airly API always return one value headers)
        private static string GetHeaderBase(IEnumerable<string> values) => Utils.GetFirstEnumarable(values);

        public static string GetHeader(HttpHeaders headers, string key)
        {
            string values = null;
            try
            {
                values = GetHeaderBase(headers.GetValues(key));
            }
            catch (Exception)
            { }
            return values;
        }
    }

    public static class ParamsValidator
    {
        public static double InfinityToDouble(double inifnity)
        {
            if (double.IsPositiveInfinity(inifnity))
                return -1;
            else if (double.IsNegativeInfinity(inifnity))
                return 0;
            else return inifnity;
        }

        public static bool AreFinity(params double[] numbers)
        {
            List<bool> finities = new List<bool>();
            foreach (var n in numbers)
                finities.Add(!double.IsInfinity(n));

            return finities.FindAll(e => e == true).Count == numbers.Length;
        }

        public static void ThrowIfInfinity(params double[] numbers)
        {
            if (!AreFinity(numbers)) throw new IndexOutOfRangeException("The specified parameters must be finity double");
        }

        public static bool CheckIfNegativeNumber(int number) => number.ToString().StartsWith("-");

        public static void ThrowIfNegativeNumber(int number)
        {
            if (CheckIfNegativeNumber(number))
                throw new InvalidOperationException("The specified number can not be negative number");
        }

        public static void ThrowIfNegativeNumberOrZero(int number)
        {
            if (number == 0 || CheckIfNegativeNumber(number))
                throw new InvalidOperationException("The specified number can not be negative number or be Zero");
        }
    }
}