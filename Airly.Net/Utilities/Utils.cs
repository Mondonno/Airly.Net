using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Reflection;
using System.Globalization;

using AirlyNet.Rest.Typings;
using AirlyNet.Common.Handling.Exceptions;
using AirlyNet.Common.Handling;

namespace AirlyNet.Utilities
{
    // General utilities class
    public static class Utils
    {
        public static List<(string, object)> GetClassProperties<T>(T classObject)
        {
            Type classType = classObject.GetType();
            PropertyInfo[] props = classType.GetProperties();
            List<(string, object)> normalizedProperties = new();

            foreach (var prop in props)
            {
                string name = prop.Name;
                MethodInfo method = prop.GetGetMethod();

                if (!prop.CanRead) continue;
                if (method == null) continue;

                object desiredValue = method.Invoke(classObject, null);
                normalizedProperties.Add((name, desiredValue));
            }

            return normalizedProperties;
        }

        public static List<List<string>> ParseQuery(dynamic query)
        {
            NumberFormatInfo numberInfo = new NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            if (query == null)
                return default;

            List<(string, object)> properties = GetClassProperties(query);
            List<List<string>> convertedQuery = new List<List<string>>();

            foreach (var op in properties)
            {
                var p = new
                {
                    Name = op.Item1,
                    Value = op.Item2
                };

                string name = p.Name;
                string value; // Converting the object value to string (without the explict type)

                if (p.Value.GetType() == typeof(double)) value = ((double)p.Value).ToString(numberInfo);
                else value = p.Value.ToString();

                List<string> constructedArray = new List<string>() { name, value };
                convertedQuery.Add(constructedArray);
            }

            return convertedQuery;
        }

        public static void ValidateKey(string key)
        {
            string toValidate = key;
            string validatedKey = toValidate.Replace(" ", "").Trim().Normalize();

            if (string.IsNullOrEmpty(toValidate) || string.IsNullOrEmpty(validatedKey) || validatedKey != toValidate)
                throw new InvalidApiKeyException();
        }

        public static string GetRoute(string url)
        {
            if (url == null) return null;
            string[] routes = url.Split('/');

            if (routes.Length == 0) return url;
            else return routes[0];
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
        private static string GetHeaderBase(IEnumerable<string> values) => values.FirstOrDefault();

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
            return numbers.ToList().Select(e => double.IsFinite(e)).ToList().Count(e => e == true) == numbers.Length;
        }

        public static void ThrowIfInfinity(params double[] numbers)
        {
            if (!AreFinity(numbers))
                throw new IndexOutOfRangeException("The specified parameters must be finity double");
        }

        private static bool CheckIfNegativeNumber(int number) => number < 0;

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