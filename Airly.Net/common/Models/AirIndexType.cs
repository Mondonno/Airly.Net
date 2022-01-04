using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirIndexType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("levels")]
        public List<AirIndexLevel> Levels { get; set; }
    }
}
