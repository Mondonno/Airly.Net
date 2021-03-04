using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AirlyAPI.Utilities
{
    // Simple JSON Parsing class utitlity for the AirlyAPI C# wrapper
    // Made by github.com/Mondonno
    public static class JsonParser
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            DateFormatString = "yyyy-MM-ddTH:mm:ss.fffK",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateParseHandling = DateParseHandling.DateTime,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static Type GetTokenType(JTokenType type) => type.GetType();
        public static string GetJTokenJson(JToken token) => token.ToString();

        public static bool CheckJsonArray(string json)
        {
            string jsonValidation = json.Trim().ToString();
            bool arrayCheck = jsonValidation.StartsWith("[") && jsonValidation.EndsWith("]");
            return arrayCheck;
        }
        public static bool CheckJsonObject(string json)
        {
            string jsonValidation = json.Trim().ToString();
            bool objectCheck = jsonValidation.StartsWith("{") && jsonValidation.EndsWith("}");
            return objectCheck;
        }
        public static bool CheckJsonValidation(string json)
        {
            bool isJsonObject = CheckJsonObject(json);
            bool isJsonArray = CheckJsonArray(json);

            bool isValidJson = isJsonObject || isJsonArray;
            return isValidJson;
        }

        public static JToken ParseJson(string json)
        {
            JObject[] finalArrayResult = new JObject[0];
            if (CheckJsonArray(json))
            {
                var arr = JArray.Parse(json);

                foreach (JObject obj in arr) ArrayUtil.ArrayPush(ref finalArrayResult, obj);
                finalArrayResult = ArrayUtil.RemoveNullValues(finalArrayResult);

                if (finalArrayResult.Length == 0) return new JArray();

                return arr;
            }
            else if (CheckJsonObject(json)) return JObject.Parse(json);
            else return new JObject();
        }

        public static T ParseToClassJSON<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default;

            string rawjson = json.ToString();
            T classment = JsonConvert.DeserializeObject<T>(rawjson, SerializerSettings);
            return classment;
        }
        public static T ParseToClassJSON<T>(JToken jsonToken) => ParseToClassJSON<T>(GetJTokenJson(jsonToken));

        public static string SerializeJSON<T>(T obj)
        {
            JsonSerializerSettings settings = SerializerSettings;
            settings.NullValueHandling = NullValueHandling.Include;

            return JsonConvert.SerializeObject((T)obj, settings);
        }
        public static string SerializeJSON(JToken json) => GetJTokenJson(json);
    }
}
