using AirlyNet.Common.Models;
using AirlyNet.Rest.Typings;

namespace AirlyNet.Common
{
    public sealed class AirlyEndpoints
    {
        public string Meta { get; } = "meta";
        public string Installations { get; } = "installations";
        public string Measurements { get; } = "measurements";

        public string Api { get; } = "airapi";
        public string Cdn { get; } = "cdn";
    }

    /// <summary>
    /// Airly Configuration, used if you want to change the default settings.
    /// </summary>
    public sealed class AirlyConfiguration
    {
        /// <summary>
        /// HTTP Agent used when requesting to API
        /// </summary>
        public string Agent { get; } = "Airly.Net";

        /// <summary>
        /// The protocol of the API request.
        /// </summary>
        public RestRequestProtocol Protocol { get; set; } = RestRequestProtocol.HTTPS;

        /// <summary>
        /// HTTP request timeout, change this if you have slow internet connection.
        /// </summary>
        public double RestRequestTimeout { get; set; } = 60000;

        /// <summary>
        /// Specifies the reaction when the resource was not found
        /// </summary>
        public AirlyNotFoundHandling NotFoundHandling { get; set; } = AirlyNotFoundHandling.Null;

        /// <summary>
        /// API Version used when requesting to Airly
        /// </summary>
        public int Version { get; set; } = 2;

        /// <summary>
        /// The main domain of Airly.
        /// </summary>
        public string Domain { get; } = "airly.eu";

        /// <summary>
        /// API domain of Airly.
        /// </summary>
        public string ApiDomain { get; private set; }

        /// <summary>
        /// CDN domain of Airly
        /// </summary>
        public string Cdn { get; private set; }

        /// <summary>
        /// Constant EndPoints of Airly.
        /// </summary>
        public AirlyEndpoints EndPoints { get; } = new AirlyEndpoints();

        public AirlyConfiguration()
        {
            ApiDomain = $"{EndPoints.Api}.{Domain}";
            Cdn = $"{EndPoints.Cdn}.{Domain}";
        }
    }
}
