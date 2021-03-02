﻿using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Net.Http;

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
        public HttpError(Exception rawError) : base("[HTTP_ERROR] New http(web) error WITH the unknown stack trace", rawError) { }
    }
    
    public class AirlyError : BaseError
    {
        public AirlyError(string content) : base(string.Format("The airly get following error.\nMessage: {0}", content)) { }
        public AirlyError(HttpError error) : base(string.Format("Airly get the following http error {0}", error.ToString())) { }
        public AirlyError(string link, string content) : base(string.Format("The airly get following error.\nMessage: {0}", content)) {
            this.HelpLink = link;
        }
    }
}

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
        public void MakeRateLimitError(AirlyResponse res, Utils utils, string customMessage = null)
        {
            string limits = utils.getHeader(res.headers, "X-RateLimit-Limit-day");
            string all = utils.calculateRateLimit(res).ToString();

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

        public void handleError(int code, AirlyResponse response)
        {
            var utils = new Utils();
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

            int limit = utils.calculateRateLimit(response);
            if (limit == 0) throw new AirlyError($"Get ratelimited by airly api\n{utils.calculateRateLimit(response)}");
            if (code > 200 && code <= 300)
            {
                if (code == 301) throw new HttpError("[REQUEST] REQUEST IS FORBIDDEN");
                throw new AirlyError("Unknown invalid request/response");
            }
            if (code >= 400 && code < 500)
            {
                if (code == 401) throw new AirlyError("The provided API Key is not valid");
                if (code == 429)
                {
                    MakeRateLimitError(utils.getHeader(response.headers, "X-RateLimit-Limit-day"), $"{utils.calculateRateLimit(response)}", "");
                    return;
                }
                HandleMalformed(rawJSON);
                throw new AirlyError($"[AIRLY_INVALID] [{response.timestamp}] {rawJSON}");
            }
            if (code >= 500 && code < 600) throw new HttpError("[AIRLY] INTERNAL PROBLEM WITH AIRLY API");
        }

        
        public RateLimitInformation GetRateLimitDetails(HttpResponseMessage res)
        {
            var utils = new Utils();
            var headers = res.Headers;

            int rateLimitDiffrent = utils.calculateRateLimit(headers);

            int perDay =  string.IsNullOrEmpty(utils.getHeader(headers, utils.XLimitName)) ? Convert.ToInt32(utils.getHeader(headers, utils.XLimitName)) : 0;
            int perDayUsed = string.IsNullOrEmpty(utils.getHeader(headers, utils.XRemainingName)) ? Convert.ToInt32(utils.getHeader(headers, utils.XRemainingName)) : 0;

            bool isLimited = utils.getRatelimit(headers);

            RateLimitInformation rateLimitInfo = new RateLimitInformation()
            {
                IsRateLimited = isLimited,
                RateLimitDiffrent = rateLimitDiffrent,
                PerDays = perDayUsed,
                PerGlobal = perDay
            };

            return rateLimitInfo;
        }

        public ErrorModel GetErrorFromJSON(JToken json)
        {
            bool errorCheck = json["errorCode"] == null;
            if (errorCheck) return null;

            JToken[] tokens = {
                json["errorCode"],
                json["message"],
                json["details"]
            };
            JObject[] errors = new Utils().convertTokens(tokens);

            string code = errors[0].ToString();
            string message = errors[1].ToString();

            JObject details = errors[2];

            int succesor;
            if (code == "INSTALLATION_REPLACED") succesor = details.SelectToken("successorId") != null ? Convert.ToInt32(details.SelectToken("successorId").ToString()) : Convert.ToInt32(null);
            else succesor = 0;

            var errorModel = new ErrorModel(message, succesor, code);
            return errorModel;
        }
    }
}
