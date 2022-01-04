using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirStandard
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("pollutant")]
        public string Pollutant { get; set; }

        [JsonProperty("averaging")]
        public string Averaging { get; set; }
        [JsonProperty("limit")]
        public double? Limit { get; set; }

        [JsonProperty("percent")]
        public double? Percent { get; set; }
    }
}
