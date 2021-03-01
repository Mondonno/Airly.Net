using System;

namespace AirlyAPI
{
    public class EndPoints
    {
        public string meta { get; } = "meta";
        public string installations { get; } = "installations";
        public string measurements { get; } = "measurements";

        public string api { get; } = "airapi";
        public string cdn { get; } = "cdn";
    }

    public class AirlyConfiguration
    {
        public string agent { get; set; } = "Airly-C#-Wrapper";
        public string protocol { get; set; } = "https://";

        public double requestTimeout { get; set; } = 60000.00;

        public int version { get; set; } = 2;
        public string domain { get; } = "airly.eu";

        public string apiDomain { get; private set; }
        public string cdn { get; private set; }

        public EndPoints endPoints { get; private set; }

        public AirlyConfiguration()
        {
            endPoints = new EndPoints();
            apiDomain = $"{endPoints.api}.{domain}";
            cdn = $"{endPoints.cdn}.{domain}";
        }
    }
}
