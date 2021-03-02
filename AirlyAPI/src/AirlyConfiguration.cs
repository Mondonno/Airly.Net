using System;

namespace AirlyAPI
{
    public class EndPoints
    {
        public string Meta { get; } = "meta";
        public string Installations { get; } = "installations";
        public string Measurements { get; } = "measurements";

        public string Api { get; } = "airapi";
        public string Cdn { get; } = "cdn";
    }

    public class AirlyConfiguration
    {
        public string Agent { get; set; } = "Airly-C#-Wrapper";
        public string Protocol { get; set; } = "https://";

        public double RestRequestTimeout { get; set; } = 60000;

        public int Version { get; set; } = 2;
        public string Domain { get; } = "airly.eu";

        public string ApiDomain { get; private set; }
        public string Cdn { get; private set; }

        public EndPoints EndPoints { get; private set; }

        public AirlyConfiguration()
        {
            EndPoints = new EndPoints();
            ApiDomain = $"{Protocol}{EndPoints.Api}.{Domain}";
            Cdn = $"{Protocol}{EndPoints.Cdn}.{Domain}";
        }
    }
}
