using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AirlyNet.Utilities
{
    public static class JsonParser
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            DateFormatString = "yyyy-MM-ddTH:mm:ss.fffK",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateParseHandling = DateParseHandling.DateTime,
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static Type GetTokenType(JTokenType type) => type.GetType();
        public static string GetJTokenJson(JToken token) => token.ToString();

        private static bool ValidateInternally(string json)
        {
            try
            {
                _ = JToken.Parse(json);
                return true;
            }catch
            {
                return false;
            }
        }

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

            bool isValidJson = isJsonObject || isJsonArray || ValidateInternally(json);
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

        public static T DeserializeJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default;

            string rawjson = json.ToString();

            T classment;
            try
            {
                classment = JsonConvert.DeserializeObject<T>(rawjson, SerializerSettings);
            }catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return default;
            }

            return classment;
        }
        public static T DeserializeJson<T>(JToken jsonToken) => DeserializeJson<T>(GetJTokenJson(jsonToken));
        public static T DeserializeJson<T> (Stream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public static string SerializeJSON(JToken jsonToken) => GetJTokenJson(jsonToken);
        public static string SerializeJSON<T>(T obj)
        {
            JsonSerializerSettings settings = SerializerSettings;
            settings.NullValueHandling = NullValueHandling.Include;

            return JsonConvert.SerializeObject(obj, settings);
        }
    }

    // depend on the json parser
    public class RestResponseParser<T>
    {
        public string Json { get; set; }
        public T Deserializated { get; protected set; }

        public RestResponseParser(string json)
        {
            Json = json;
            Refresh();
        }

        public void Refresh() => Deserializated = Parse();
        private T Parse()
        {
            if (Json == null) return default;

            var parsedJson = JsonParser.DeserializeJson<T>(Json);
            if (parsedJson == null) return default;
            else return parsedJson;
        }
    }
}
