
namespace AirlyAPI
{
    public sealed class AirlyEndPoints
    {
        public string Meta { get; } = "meta";
        public string Installations { get; } = "installations";
        public string Measurements { get; } = "measurements";

        public string Api { get; } = "airapi";
        public string Cdn { get; } = "cdn";
    }

    public sealed class AirlyConfiguration
    {
        // "Airly-C#-Wrapper"
        public string Agent { get; set; } = "AirlyAPI C# Wrapper";
        public string Protocol { get; set; } = "https";

        public double RestRequestTimeout { get; set; } = 60000;
        public AirlyNotFoundHandling NotFoundHandling { get; set; } = AirlyNotFoundHandling.Null;

        public int Version { get; set; } = 2;
        public string Domain { get; } = "airly.eu";

        public string ApiDomain { get; private set; }
        public string Cdn { get; private set; }

        public AirlyEndPoints EndPoints { get; } = new AirlyEndPoints();

        public AirlyConfiguration()
        {
            ApiDomain = $"{EndPoints.Api}.{Domain}";
            Cdn = $"{EndPoints.Cdn}.{Domain}";
        }
    }
}
