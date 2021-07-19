using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

using AirlyNet.Utilities;
using AirlyNet.Rest.Typings;
using AirlyNet.Handling.Errors;
using System.Net;

namespace AirlyNet.Handling
{
    public class ErrorModel
    {
        public bool HaveError { get; set; }

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
        protected HttpResponseMessage HttpResponse { get; set; }

        private string Json { get; set; }

        public ErrorDeserializer(HttpResponseMessage httpResponse,string json)
        {
            Json = json;
            HttpResponse = httpResponse;
        }

        private string GetSuccesor(HttpHeaders httpHeaders)
        {
            var header = RestUtil.GetHeader(httpHeaders, "Location");
            string value = header != null ? header : null;
            return value;
        }

        public ErrorModel Deserialize()
        {
            var deserializedError = JsonDeserialize(this.Json);
            if (!deserializedError.IsError) return new()
            {
                HaveError = false,
                Code = null,
                ErrorContent = null,
                Succesor = null
            };

            var errorContent = deserializedError.RawError;
            var errorInformation = new ErrorModel()
            {
                HaveError = true,
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

        public static void MakeRateLimitError(RawRestResponse res, string customMessage = null)
        {
            var headers = res.HttpResponse.Headers;

            string limits = RestUtil.GetHeader(headers, "X-RateLimit-Limit-day");
            string all = RatelimitsUtil.CalculateRateLimit(headers).ToString();

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
            { throw new HttpError($"Server sent possible malformed response\n{ex.Message}"); }
        }

        public void Refresh(string newJson) => Json = newJson;
    }

    public class Handler
    {
        private string ResponseJson { get; set; }

        private HttpResponseHeaders ResponseHeaders { get; set; }
        private HttpStatusCode HttpResponseCode { get; set; }

        public JsonErrorHandler JsonHandler { get; set; }

        public Handler(HttpResponseMessage responseMessage, string Json = null)
        {
            ResponseHeaders = responseMessage.Headers;
            HttpResponseCode = responseMessage.StatusCode;

            ResponseJson = Json ?? string.Empty;
            JsonHandler = new(ResponseJson);
        }

        public void HandleResponseCode(int? responseCode = null) => InternalHandleResponseCode(responseCode);

        protected void InternalHandleResponseCode(int? responseCode = null)
        {
            int statusCode = responseCode ?? (int) HttpResponseCode;
            string rawJson = ResponseJson;

            JsonErrorHandler handler = new(rawJson);
            HttpResponseHeaders headers = ResponseHeaders;

            if (statusCode > 0 && statusCode <= 200) return; // all ok, returning
            if (statusCode == 404)
            {
                handler.HandleMalformed();
                return;
            };

            int? limit = RatelimitsUtil.CalculateRateLimit(headers);

            if (limit == 0) throw new AirlyError($"Get ratelimited by airly api\n{RatelimitsUtil.CalculateRateLimit(headers)}");
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
                    RateLimitThrower.MakeRateLimitError(RestUtil.GetHeader(headers, "X-RateLimit-Limit-day"), $"{RatelimitsUtil.CalculateRateLimit(headers)}", "");
                    return;
                }
                handler.HandleMalformed();

                throw new AirlyError($"[{DateTime.Now}] Invalid response code {rawJson}");
            }
            if (statusCode >= 500 && statusCode < 600) throw new HttpError("The API throwed 500, internal Airly API problem");
        }

        public void Refersh(string json)
        {
            ResponseJson = json;
            JsonHandler = new(ResponseJson);
        }
    }
}
