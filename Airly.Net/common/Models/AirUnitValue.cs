using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirUnitValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double? Value { get; set; }
    }
}
