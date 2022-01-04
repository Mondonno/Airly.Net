using System;
using Newtonsoft.Json;
using AirlyNet.Utilities;

namespace AirlyNet.Common.Models
{
    /// <summary>
    /// Main Location object used when interacting with Airly.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Create new location instance by passing Latidude nad Longatidude
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public Location(double lat, double lng)
        {
            ParamsValidator.ThrowIfInfinity(lat, lng);

            Lat = lat;
            Lng = lng;
        }

        [JsonProperty("latitude")]
        public double Lat { get; set; }

        [JsonProperty("longitude")]
        public double Lng { get; set; }

        public static bool operator ==(Location one, Location two) => (!Equals(one, null) && !Equals(two, null)) ? one.Lng == two.Lng && one.Lat == two.Lat : Equals(one, two);

        public static bool operator !=(Location one, Location two) => (!Equals(one, null) && !Equals(two, null)) ? one.Lng != two.Lng || one.Lat != two.Lat : !Equals(one, two);

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => $"{Lat} {Lng}";
    }
}
