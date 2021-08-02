﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Newtonsoft.Json.Linq;

using AirlyNet.Rest.Typings;
using AirlyNet.Handling.Errors;
using AirlyNet.Handling;

namespace AirlyNet.Utilities
{
    public static class Utils
    { 
        // Simple coping the one dictonary to another without the overwriting
        public static void CopyDictonaryValues(ref IDictionary<object, object> target, IDictionary<object, object> toCopyInto)
        {
            foreach (var pair in toCopyInto)
                target.Add(pair.Key, pair.Value);
        }

        // Simple converting JTokens to the JObjects by exclipting the C# types
        public static JObject[] ConvertTokens(JToken[] tokens)
        {
            JObject[] jObjects = new JObject[0];
            foreach (var token in tokens)
            {
                CollectionsUtil.ArrayPush(ref jObjects, (JObject)token);
            }
            return jObjects;
        }

        // Formmatting web query
        public static string FormatQuery(string query)
        {
            if (query.StartsWith("?")) query = query.Remove(0, 1);

            string virtualHost = "https://example.com";
            Uri constructedQuery = new Uri(string.Format("{0}{1}{2}", virtualHost, "/",
                    (!string.IsNullOrEmpty(query) ? string.Format("?{0}",
                    query.Replace("#", "%23").Replace("@", "%40")) // Replacing # and @ to the own URL code (Uri does not support # and @ in query)
                    : ""))); // no need to parse the Query (eg. % === %25) (the Uri class do this for me)

            string Query = constructedQuery.Query;
            return Query;
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

                Type propType = prop.PropertyType; // cs-unused
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

        public static string[][] ParseQuery(dynamic query)
        {
            NumberFormatInfo numberInfo = new NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            if (query == null) return new string[0][];
            List<NormalizedProperty> properties = GetClassProperties(query);

            string[][] convertedQuery = { };
            foreach (var p in properties)
            {
                string name = p.name;

                string value = ""; // Converting the object value to string (without the explict type)
                if (IsDouble(p.value)) value = ((double)p.value).ToString(numberInfo);
                else value = p.value.ToString();

                _ = value == null ? value = "" : null;

                string[] constructedArray = { name, value };
                CollectionsUtil.ArrayPush(ref convertedQuery, constructedArray);
            }

            return convertedQuery;
        }

        public static T GetFirstEnumarable<T>(IEnumerable<T> enumarable) => enumarable.First((e) => true);

        public static object InvokeMethod<T>(T obj, string methodName, object[] parameters)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(methodName);

            object result = method.Invoke(obj, parameters);
            return result;
        }

        public static bool IsPropertyExists<T>(T obj, string name)
        {
            Type type = obj.GetType();
            PropertyInfo result = type.GetProperty(name);

            return result != null;
        }

        public static bool IsAggregated(Exception ex)
        {
            try
            {
                _ = (AggregateException)ex;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            return true;
        }

        public static string GetVersion(int version, bool slash) => $"{(slash ? "/" : string.Empty)}v{version}{(slash ? "/" : string.Empty)}";

        public static string GetInners(AggregateException ag)
        {
            List<string> Messages = new List<string>();
            foreach (var exception in ag.InnerExceptions) Messages.Add(exception.Message);

            return CollectionsUtil.JoinList(Messages, "\n");
        }

        public static void ValidateKey(string key)
        {
            // Simple airly key validation (eg. key of whitespaces)
            string toValidate = key;
            string validatedKey = toValidate.Replace(" ", "").Trim().Normalize();

            if (string.IsNullOrEmpty(toValidate) || string.IsNullOrEmpty(validatedKey) || validatedKey != toValidate) throw new InvalidApiKeyError();
            else return;
        }

        public static string GetRoute(string url)
        {
            if (url == null) return null;
            string[] routes = url.Split('/');

            if (routes.Length == 0 || (routes.Length == 1 && routes[0] == url)) return url;
            else return routes[0].ToString();
        }

        [Obsolete]
        public static string ReplaceDashUpper(string str)
        {
            string finalString = "";
            string[] strs = str.Split('-');

            if (strs.Length == 0 || (strs.Length == 1 && (strs[0] == str))) return str;
            foreach (string nm in strs)
            {
                finalString += string.Format("{0}{1}", nm[0].ToString().ToUpper(), nm.Remove(0, 1));
            }
            return finalString;
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

                return rateLimitCheck;
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
            foreach (var n in numbers) finities.Add(!double.IsInfinity(n));

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