using System;
using AirlyNet.Utilities;

namespace AirlyNet.Common.Models
{
    /// <summary>
    /// Location area, created from to Location objects.
    /// </summary>
    public class LocationArea
    {
        public LocationArea(Location sw, Location ne)
        {
            Sw = sw ?? throw new ArgumentNullException("sw");
            Ne = ne ?? throw new ArgumentNullException("ne");
        }

        public Location Sw { get; set; }
        public Location Ne { get; set; }

        public bool Contains(Location location) => GeoUtil.Contains(location.Lat, Sw.Lat, Ne.Lat) && GeoUtil.Contains(location.Lng, Sw.Lng, Ne.Lng);

        public Location GetBarycenter() => new Location(
            GeoUtil.GetMidpoint(Sw.Lat, Ne.Lat),
            GeoUtil.GetMidpoint(Sw.Lng, Ne.Lng)
        );

        public static bool operator ==(LocationArea one, LocationArea two) => one != null && two != null ? one.Ne == two.Ne && one.Sw == two.Sw : one == two;

        public static bool operator !=(LocationArea one, LocationArea two) => one != null && two != null ? one.Ne != two.Ne || one.Sw != two.Sw : one != two;

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => $"{Sw.Lat} {Sw.Lng}\n{Ne.Lat} {Ne.Lng}";
    }
}
