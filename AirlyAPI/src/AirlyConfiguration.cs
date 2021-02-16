using System;

namespace AirlyAPI.src
{
    public class AirlyConfiguration
    {
        public string domain { get; set; } = "airly.eu";

        public string apiDomain { get; set; }
        public string cdn { get; set; }

        public int version { get; set; } = 2;

        public AirlyConfiguration()
        {
            apiDomain = $"airapi.{domain}";
            cdn = $"cdn.{domain}";
        }
    }
}
