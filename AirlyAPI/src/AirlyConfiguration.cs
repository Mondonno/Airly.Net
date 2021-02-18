using System;

namespace AirlyAPI
{
    public class AirlyRawConfiguration
    {
        // ...
    }

    public class AirlyConfiguration : AirlyRawConfiguration
    {
        public string agent { get; } = "Airly-C#-Wrapper";
        public string protocol { get; set; } = "https://";

        public string domain { get; set; } = "airly.eu";
        public AirlyLanguage language { get; set; } = AirlyLanguage.en;

        public string apiDomain { get; set; }
        public string cdn { get; set; }

        public int version { get; set; } = 2;

        public dynamic endPoints = new
        {
            meta = "meta",
            installations = "installations",
            measurements = "measurements",
            api = "airapi",
            cdn = "cdn"
        };

        public AirlyConfiguration()
        {
            apiDomain = $"{endPoints.api}.{domain}";
            cdn = $"{endPoints.cdn}.{domain}";
        }
    }
}
