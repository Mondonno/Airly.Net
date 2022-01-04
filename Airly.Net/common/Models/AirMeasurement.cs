using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirMeasurement
    {
        [JsonProperty("current")]
        public AirSingleMeasurement Current { get; set; }

        [JsonProperty("history")]
        public List<AirSingleMeasurement> History { get; set; }
        [JsonProperty("forecast")]
        public List<AirSingleMeasurement> Forecast { get; set; }
    }
}
