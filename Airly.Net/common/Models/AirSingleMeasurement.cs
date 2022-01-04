using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirSingleMeasurement
    {
        [JsonProperty("fromDateTime")]
        public DateTime FromDateTime { get; set; }
        [JsonProperty("tillDateTime")]
        public DateTime TillDateTime { get; set; }

        [JsonProperty("values")]
        public List<AirUnitValue> Values { get; set; }
        [JsonProperty("indexes")]
        public List<AirIndex> Indexes { get; set; }
        [JsonProperty("standards")]
        public List<AirStandard> Standards { get; set; }
    }
}
