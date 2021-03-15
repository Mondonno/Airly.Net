using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

using AirlyAPI.Utilities;
using AirlyAPI.Rest.Typings;
using AirlyAPI.Handling.Errors;

namespace AirlyAPI.Handling
{
    public class ErrorModel
    {
        public string Code { get; set; }
        public string ErrorContent { get; set; }
        public string Succesor { get; set; }
        public JToken ErrorDetails { get; set; }
    }

    public class ErrorInformation
    {
        public bool IsError { get; set; }
        public JToken[] RawError { get; set; }
    }

    public class ErrorDeserializer
    {
        private Utils util { get; set; } = new();
        protected HttpResponseMessage HttpResponse { get; set; }

        private string Json { get; set; }

        public ErrorDeserializer(HttpResponseMessage httpResponse,string json)
        {
            Json = json;
            HttpResponse = httpResponse;
        }

        private string GetSuccesor(HttpHeaders httpHeaders)
        {
            var header = util.GetHeader(httpHeaders, "Location");
            string value = header != null ? header : null;
            return value;
        }

        public ErrorModel Deserialize()
        {
            var deserializedError = JsonDeserialize(this.Json);
            if (!deserializedError.IsError) return new()
            {
                Code = null,
                ErrorContent = null,
                Succesor = null
            };

            var errorContent = deserializedError.RawError;
            var errorInformation = new ErrorModel()
            {
                Code = (string)errorContent[0],
                ErrorContent = (string)errorContent[1],
                ErrorDetails = errorContent[2],
                Succesor = GetSuccesor(HttpResponse.Headers)
            };

            return errorInformation;
        }

        private ErrorInformation JsonDeserialize(string Json)
        {
            var parsedResponseJson = JsonParser.ParseJson(Json);
            var errorCode = parsedResponseJson["errorCode"];

            if (errorCode == null) return new()
            {
                IsError = false
            };

            var jsonErrorTokens = new JToken[] {
                parsedResponseJson["errorCode"],
                parsedResponseJson["message"],
                parsedResponseJson["details"]
            };

            return new() {
                IsError = true,
                RawError = jsonErrorTokens
            };
        }
    }

    public static class RateLimitThrower
    {
        public static void MakeRateLimitError(string limits, string all, string customMessage)
        {
            string errorMessage = $"Your api key get ratelimited for this day/minut/secound by Airly API\n{customMessage ?? ""}\n{all}/{limits}";
            AirlyError error = new AirlyError(errorMessage);
            error.Data.Add("Ratelimited", true);

            throw error;
        }
        public static void MakeRateLimitError(RawRestResponse res, Utils utils, string customMessage = null)
        {
            var headers = res.HttpResponse.Headers;

            string limits = utils.GetHeader(headers, "X-RateLimit-Limit-day");
            string all = utils.CalculateRateLimit(headers).ToString();

            MakeRateLimitError(limits, all, customMessage);
        }
    }

    public class JsonErrorHandler
    {
        public string Json { get; set; }
        public JsonErrorHandler(string Json) => Refresh(Json);

        public bool IsMalformedResponse()
        {
            try
            { HandleMalformed(); }
            catch (Exception)
            { return true; } 

            return false;
        }

        public void HandleMalformed()
        {
            try
            { JsonParser.ParseJson(Json); }
            catch (Exception ex)
            { throw new HttpError($"[AIRLY] POSSIBLE MALFORMED RESPONSE\n{ex.Message}"); }
        }

        public void Refresh(string newJson) => Json = newJson;
    }

    public class Handler
    {
        private string ResponseJson { get; set; }

        private HttpResponseHeaders ResponseHeaders { get; set; }
        public JsonErrorHandler JsonHandler { get; set; }

        public Handler(HttpResponseMessage responseMessage, string Json = null)
        {
            ResponseHeaders = responseMessage.Headers;
            ResponseJson = Json ?? string.Empty;
            JsonHandler = new(ResponseJson);
        }

        public void HandleResponseCode() => InternalHandleResponseCode();
        protected void InternalHandleResponseCode()
        {
            int statusCode = 1;
            string rawJson = ResponseJson;

            Utils utils = new();
            JsonErrorHandler handler = new(rawJson);
            HttpResponseHeaders headers = ResponseHeaders;

            if (statusCode > 0 && statusCode <= 200) return; // all ok, returning
            if (statusCode == 301)
            {
                handler.HandleMalformed();
                return;
            }
            if (statusCode == 404)
            {
                handler.HandleMalformed();
                return;
            };

            int? limit = utils.CalculateRateLimit(headers);

            if (limit == 0) throw new AirlyError($"Get ratelimited by airly api\n{utils.CalculateRateLimit(headers)}");
            if (statusCode > 200 && statusCode <= 300)
            {
                if (statusCode == 301)
                {
                    handler.HandleMalformed();
                    return;
                }
                throw new AirlyError("Unknown invalid request/response");
            }
            if (statusCode >= 400 && statusCode < 500)
            {
                if (statusCode == 401) throw new AirlyError("The provided API Key is not valid");
                if (statusCode == 429)
                {
                    RateLimitThrower.MakeRateLimitError(utils.GetHeader(headers, "X-RateLimit-Limit-day"), $"{utils.CalculateRateLimit(headers)}", "");
                    return;
                }
                handler.HandleMalformed();
                throw new AirlyError($"[AIRLY_INVALID] [{DateTime.Now}] {rawJson}");
            }
            if (statusCode >= 500 && statusCode < 600) throw new HttpError("[AIRLY] INTERNAL PROBLEM WITH AIRLY API");
        }

        public void Refersh(string json)
        {
            ResponseJson = json;
            JsonHandler = new(ResponseJson);
        }
    }
}
