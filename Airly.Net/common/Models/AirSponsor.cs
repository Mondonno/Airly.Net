using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirSponsor
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("logo")]
        public Uri Logo { get; set; }
        [JsonProperty("link")]
        public Uri Link { get; set; }
    }
}
