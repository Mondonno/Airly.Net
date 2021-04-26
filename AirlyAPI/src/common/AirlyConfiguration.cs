
namespace AirlyAPI
{
    public sealed class AirlyEndpoints
    {
        public string Meta { get; } = "meta";
        public string Installations { get; } = "installations";
        public string Measurements { get; } = "measurements";

        public string Api { get; } = "airapi";
        public string Cdn { get; } = "cdn";
    }

    public sealed class AirlyConfiguration
    {
        public string Agent { get; } = "Airly.Net";
        public string Protocol { get; set; } = "https";

        public double RestRequestTimeout { get; set; } = 60000;
        public AirlyNotFoundHandling NotFoundHandling { get; set; } = AirlyNotFoundHandling.Null;

        public int Version { get; set; } = 2;
        public string Domain { get; } = "airly.eu";

        public string ApiDomain { get; private set; }
        public string Cdn { get; private set; }

        public AirlyEndpoints EndPoints { get; } = new AirlyEndpoints();

        public AirlyConfiguration()
        {
            ApiDomain = $"{EndPoints.Api}.{Domain}";
            Cdn = $"{EndPoints.Cdn}.{Domain}";
        }
    }
}
