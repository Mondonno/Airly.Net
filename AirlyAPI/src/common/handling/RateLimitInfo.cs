using System;
using System.Net.Http.Headers;
using System.Net.Http;

using AirlyAPI.Utilities;

namespace AirlyAPI.Handling
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
            Limit = Convert.ToInt32(Util.GetHeader(httpHeaders, Util.XLimitName) ?? null);
            Remain = Convert.ToInt32(Util.GetHeader(httpHeaders, Util.XRemainingName) ?? null);
            Diffrence = Util.CalculateRateLimit(Remain, Limit);
            IsRateLimited = Diffrence == 0;
        }
        public RateLimitInfo(HttpResponseMessage response) : this(response.Headers) { }
        public RateLimitInfo(RawResponse response) : this(response.response.Headers) { }
    }
}
