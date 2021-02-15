// Simple error models and handling errors logic here!

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace AirlyAPI
{
    // Making simple error basement
    [Serializable]
    public class BaseError : Exception
    {
        public BaseError() : base() { }
        public BaseError(string message) : base(message) { }
        public BaseError(string message, Exception innerException) : base(message, innerException) { }
        public BaseError(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class HttpError : BaseError
    {
        public HttpError(string content) : base(string.Format("[HTTP_ERROR] New http (web) error {0}", content)) { }
    }
    
    public class AirlyError : BaseError
    {
        public AirlyError(string content) : base(string.Format("The airly get following error.\nMessage: {0}", content)) { }
        public AirlyError(HttpError error) : base(string.Format("Airly get the following http error {0}", error.ToString())) { }
    }
}

namespace AirlyAPI.handling
{
    public class Handler
    {
        public void makeRateLimitError(string limits, string all, string customMessage) {
            string errorMessage = $"Your api key get ratelimited for this day/minut/secound by Airly API\n{customMessage ?? ""}\n{all}/{limits}";
            AirlyError error = new AirlyError(errorMessage);

            throw error;
        }
        public void handleError(int code, AirlyResponse response)
        {
            var utils = new Utils();
            if (code > 0 && code <= 200) return;

            int limit = utils.calculateRateLimit(response);
            if (limit == 0) throw new AirlyError($"Get ratelimited by airly api\n{utils.calculateRateLimit(response)}");
            if (code > 200 && code <= 300)
            {
                if (code == 301) throw new HttpError("[REQUEST] REQUEST IS FORBIDDEN");
                throw new AirlyError("Unknown invalid request");
            }
            if (code >= 400 && code < 500)
            {
                if (code == 401) throw new AirlyError("The provided API Key is not valid");
                if (code == 429)
                {
                    makeRateLimitError(utils.getHeader(response.headers, "X-RateLimit-Limit-day"), $"{utils.calculateRateLimit(response)}", "");
                    return;
                }
                try
                {
                    Utils.ParseJson(response.rawJSON);
                }
                catch (Exception ex)
                {
                    throw new HttpError($"[AIRLY] POSSIBLE MALFORMED RESPONSE\n{ex.Message}");
                }
                throw new AirlyError($"[AIRLY_INVALID] [{response.timestamp.ToString()}] {response.rawJSON}");
            }
            if (code >= 500 && code < 600) throw new HttpError("[AIRLY] INTERNAL PROBLEM WITH AIRLY API");
        }
        public void getErrorFromJSON(JObject json)
        {
            bool errorCheck = json["errorCode"] == null;
            if (errorCheck) return;
            JObject[] errors = {
                (JObject) json["errorCode"],
                (JObject) json["message"],
                (JObject) json["details"]
            };
            string message = errors[1].ToString();

        }
    }
}
