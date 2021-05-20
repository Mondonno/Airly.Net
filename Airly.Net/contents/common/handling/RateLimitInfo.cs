using System;
using System.Net.Http.Headers;
using System.Net.Http;

using AirlyNet.Utilities;
using AirlyNet.Rest.Typings;

namespace AirlyNet.Handling
{
    public class RateLimitInfo
    {
        private Utils Util { get; set; }

        public bool IsRateLimited { get; set; }
        public int? Limit { get; set; }
        public int? Remain { get; set; }
        public int? Diffrence { get; set; }

        public RateLimitInfo(HttpHeaders httpHeaders)
        {
            string limit = Util.GetHeader(httpHeaders, XLimitName) ?? null;
            string remain = Util.GetHeader(httpHeaders, XRemainingName) ?? null;

            Limit = limit != null ? Convert.ToInt32(limit) : null;
            Remain = remain != null ? Convert.ToInt32(remain) : null;
            Diffrence = Util.CalculateRateLimit(Remain, Limit);
            IsRateLimited = Diffrence == 0 || Diffrence == null; 
        }

        public RateLimitInfo(HttpResponseMessage response) : this(response.Headers) { }
        public RateLimitInfo(RawRestResponse response) : this(response.HttpResponse.Headers) { }

        public static string XRemainingName = "X-RateLimit-Remaining-day";
        public static string XLimitName = "X-RateLimit-Limit-day";

    }
}
