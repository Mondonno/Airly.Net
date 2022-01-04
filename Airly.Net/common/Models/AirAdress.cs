using System;
using Newtonsoft.Json;

namespace AirlyNet.Common.Models
{
    public class AirAddress
    {
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("displayAddress1")]
        public string DisplayAddressOne { get; set; }
        [JsonProperty("displayAddress2")]
        public string DisplayAddressTwo { get; set; }
    }
}
