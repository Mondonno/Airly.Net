using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirInstallation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        // can not check the locationId is always avaible because of this isn't documentated
        [JsonProperty("locationId")]
        public int? LocationId { get; set; }

        [JsonProperty("address")]
        public AirAddress Address { get; set; }
        [JsonProperty("location")]
        public Location Location { get; set; }
        [JsonProperty("sponsor")]
        public AirSponsor Sponsor { get; set; }

        [JsonProperty("elevation")]
        public double Elevation { get; set; }
        [JsonProperty("airly")]
        public bool Airly { get; set; }
    }

}
