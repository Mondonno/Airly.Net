using System;

namespace AirlyAPI
{
    public class AirlyRawConfiguration
    {
        // ...
    }

    public class EndPoints
    {
        public string meta { get; } = "meta";
        public string installations { get; } = "installations";
        public string measurements { get; } = "measurements";

        public string api { get; } = "airapi";
        public string cdn { get; } = "cdn";
    }

    public class AirlyConfiguration : AirlyRawConfiguration
    {
        public string agent { get; } = "Airly-C#-Wrapper";
        public string protocol { get; set; } = "https://";

        public string domain { get; set; } = "airly.eu";
        public AirlyLanguage language { get; set; } = AirlyLanguage.en;

        public string apiDomain { get; private set; }
        public string cdn { get; private set; }

        public int version { get; } = 2;

        public EndPoints endPoints { get; private set; }

        public AirlyConfiguration()
        {
            endPoints = new EndPoints();
            apiDomain = $"{endPoints.api}.{domain}";
            cdn = $"{endPoints.cdn}.{domain}";
        }
    }
}
