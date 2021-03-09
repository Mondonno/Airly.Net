﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Reflection;
using System.Globalization;

using Newtonsoft.Json.Linq;
using AirlyAPI.Handling;

namespace AirlyAPI.Utilities
{
    public sealed class Utils
    {
        // The ratelimits headers names
        public string XRemainingName = "X-RateLimit-Remaining-day";
        public string XLimitName = "X-RateLimit-Limit-day";

        // Checking if the ratelimit is reached
        private bool GetRateLimitBase(string XRemaining, string XLimit)
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
        public bool GetRatelimit(HttpResponseHeaders headers)
        {
            string XRemaining = GetHeader(headers, this.XRemainingName);
            string XLimit = GetHeader(headers, this.XLimitName);

            object rateLimit = GetRateLimitBase(XRemaining, XLimit);
            return (bool)rateLimit;
        }

        // The method to get the ratlimits from the response message
        public bool GetRatelimit(AirlyResponse response)
        {
            HttpResponseHeaders headers = response.headers;

            string XRemaining = GetHeader(headers, this.XRemainingName);
            string XLimit = GetHeader(headers, this.XLimitName);

            object rateLimit = GetRateLimitBase(XRemaining, XLimit);
            return (bool)rateLimit;
        }

        public int? CalculateRateLimit(int? XRemaining, int? XLimit)
        {
            if (XRemaining == null || XLimit == null) return null;

            int? calculated = XLimit - (XLimit - XRemaining);
            return calculated;
        }

        // Calculating the ratelimits diffrents
        public int? CalculateRateLimit(HttpResponseHeaders responseHeaders)
        {
            var headers = responseHeaders;

            string XRemaining = GetHeader(headers, this.XRemainingName);
            string XLimit = GetHeader(headers, this.XLimitName);

            if (XRemaining == null || XLimit == null) return null;

            int cnv1 = Convert.ToInt32(XRemaining);
            int cnv2 = Convert.ToInt32(XLimit);

            return CalculateRateLimit(cnv1, cnv2);
        }
        public int? CalculateRateLimit(AirlyResponse res) => CalculateRateLimit(res.headers);

        private string GetHeaderBase(IEnumerable<string> values) => this.GetFirstEnumarable(values);

        // For response headers
        // Getting the header first value because the headers can have multiple values (Airly API always return one value headers)
        public string GetHeader(HttpHeaders headers, string key)
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

            return ArrayUtil.JoinList(Messages,"\n");
        }
        public static void ValidateKey(string key)
        {
            // Simple airly key validation (eg. key of whitespaces)
            string toValidate = key;

            AirlyError invalidToken = new AirlyError("Provided airly api key is invalid");
            invalidToken.Data.Add("Token", false);

            string validatedKey = toValidate.Replace(" ", "").Trim().Normalize();

            if (string.IsNullOrEmpty(toValidate) || string.IsNullOrEmpty(validatedKey) || validatedKey != toValidate) throw invalidToken;
            else return;
        }

        public static string GetRoute(string url)
        {
            if (url == null) return null;
            string[] routes = url.Split('/');

            if (routes.Length == 0 || (routes.Length == 1 && routes[0] == url)) return url;

            return routes[0].ToString();
        }

        // Simple coping the one dictonary to another without the overwriting
        public void CopyDictonaryValues(ref IDictionary<object, object> target, IDictionary<object, object> toCopyInto)
        {
            foreach (var pair in toCopyInto)
                target.Add(pair.Key, pair.Value);
        }

        // Simple converting JTokens to the JObjects by exclicting the C# types
        public JObject[] ConvertTokens(JToken[] tokens)
        {
            JObject[] jObjects = new JObject[0];
            foreach (var token in tokens)
            {
                ArrayUtil.ArrayPush(ref jObjects, (JObject)token);
            }
            return jObjects;
        }

        // Formmatting web query
        public string FormatQuery(string query)
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

        public string ReplaceDashUpper(string str)
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

        public bool IsDouble(object obj)
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

        public string[][] ParseQuery(dynamic query)
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
                ArrayUtil.ArrayPush(ref convertedQuery, constructedArray);
            }

            return convertedQuery;
        }

        public T GetFirstEnumarable<T>(IEnumerable<T> enumarable) => enumarable.First((e) => true);

        public object InvokeMethod<T>(T obj, string methodName, object[] parameters)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(methodName);

            object result = method.Invoke(obj, parameters);
            return result;
        }

        public bool IsPropertyExists<T>(T obj, string name)
        {
            Type type = obj.GetType();
            PropertyInfo result = type.GetProperty(name);

            return result != null;
        }
    }
}