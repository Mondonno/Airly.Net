using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirIndexLevel
    {
        [JsonProperty("maxValue")]
        public int? MaxValue { get; set; }
        [JsonProperty("minValue")]
        public int? MinValue { get; set; }

        [JsonProperty("values")]
        public string Values { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
    }
}
