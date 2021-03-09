using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using AirlyAPI.Utilities;

namespace AirlyAPI.Handling
{
    public class ErrorModel
    {
        public ErrorModel(string errorContent, int? succesor, string code)
        {
            this.errorContent = errorContent;
            this.succesor = succesor;
            this.code = code;
        }

        public string code { get; set; }
        public string errorContent { get; set; }
        public int? succesor { get; set; }
    }

    public class Handler
    {
        public void MakeRateLimitError(string limits, string all, string customMessage) {
            string errorMessage = $"Your api key get ratelimited for this day/minut/secound by Airly API\n{customMessage ?? ""}\n{all}/{limits}";
            AirlyError error = new AirlyError(errorMessage);
            error.Data.Add("Ratelimited", true);

            throw error;
        }
        public void MakeRateLimitError(RawResponse res, Utils utils, string customMessage = null)
        {
            var headers = res.response.Headers;

            string limits = utils.GetHeader(headers, "X-RateLimit-Limit-day");
            string all = utils.CalculateRateLimit(headers).ToString();

            MakeRateLimitError(limits, all, customMessage);
        }

        public bool IsMalformedResponse(string JsonString)
        {
            try
            { HandleMalformed(JsonString.ToString()); }
            catch (Exception)
            { return true; } // Malformed

            return false; // Normal response, non malformed
        }

        // Handling the malformed request and throwing a new error
        private void HandleMalformed(string rawJSON)
        {
            try
            { JsonParser.ParseJson(rawJSON); }
            catch (Exception ex)
            { throw new HttpError($"[AIRLY] POSSIBLE MALFORMED RESPONSE\n{ex.Message}"); }
        }

        public void HandleError(int code, RawResponse response)
        {
            var utils = new Utils();

            var headers = response.response.Headers;
            string rawJSON = response.rawJSON;

            if (code > 0 && code <= 200) return; // all ok, returning

            // MOVED_PERMANETLY response code 301
            if(code == 301) {
                HandleMalformed(rawJSON);
                return;
            }
            if (code == 404) {
                HandleMalformed(rawJSON);
                return;
            }; // Passing null on the 404

            int? limit = utils.CalculateRateLimit(headers);
            if (limit == 0) throw new AirlyError($"Get ratelimited by airly api\n{utils.CalculateRateLimit(headers)}");
            if (code > 200 && code <= 300)
            {
                if(code == 301)
                {
                    HandleMalformed(rawJSON);
                    return;
                }
                throw new AirlyError("Unknown invalid request/response");
            }
            if (code >= 400 && code < 500)
            {
                if (code == 401) throw new AirlyError("The provided API Key is not valid");
                if (code == 429)
                {
                    MakeRateLimitError(utils.GetHeader(headers, "X-RateLimit-Limit-day"), $"{utils.CalculateRateLimit(headers)}", "");
                    return;
                }
                HandleMalformed(rawJSON);
                throw new AirlyError($"[AIRLY_INVALID] [{DateTime.Now}] {rawJSON}");
            }
            if (code >= 500 && code < 600) throw new HttpError("[AIRLY] INTERNAL PROBLEM WITH AIRLY API");
        }

        public ErrorModel GetErrorFromJson(JToken json)
        {
            bool errorCheck = json["errorCode"] == null;
            if (errorCheck) return null;

            JToken[] tokens = {
                json["errorCode"],
                json["message"],
                json["details"]
            };
            JObject[] errors = new Utils().ConvertTokens(tokens);

            string code = errors[0].ToString();
            string message = errors[1].ToString();

            JObject details = errors[2];

            // Checking the succesors of the error
            int? succesor;
            if (code == "INSTALLATION_REPLACED" && details["successorId"] != null) succesor = Convert.ToInt32(details["successorId"].ToString());
            else succesor = null;

            var errorModel = new ErrorModel(message, succesor, code);
            return errorModel;
        }
    }
}
