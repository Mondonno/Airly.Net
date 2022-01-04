using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirIndex
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double? Value { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("advice")]
        public string Advice { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
    }
}
