using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirMeasurementType
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }
}
