using System;
using AirlyNet.Utilities;

namespace AirlyNet.Common.Models
{
    /// <summary>
    /// Location area, created from to Location objects.
    /// </summary>
    public class AirLocationArea
    {
        public AirLocationArea(AirLocation sw, AirLocation ne)
        {
            Sw = sw ?? throw new ArgumentNullException("sw");
            Ne = ne ?? throw new ArgumentNullException("ne");
        }

        public AirLocation Sw { get; set; }
        public AirLocation Ne { get; set; }

        public bool Contains(AirLocation location) => GeoUtil.Contains(location.Lat, Sw.Lat, Ne.Lat) && GeoUtil.Contains(location.Lng, Sw.Lng, Ne.Lng);

        public AirLocation GetBarycenter() => new AirLocation(
            GeoUtil.GetMidpoint(Sw.Lat, Ne.Lat),
            GeoUtil.GetMidpoint(Sw.Lng, Ne.Lng)
        );

        public static bool operator ==(AirLocationArea one, AirLocationArea two) => one != null && two != null ? one.Ne == two.Ne && one.Sw == two.Sw : one == two;

        public static bool operator !=(AirLocationArea one, AirLocationArea two) => one != null && two != null ? one.Ne != two.Ne || one.Sw != two.Sw : one != two;

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => $"{Sw.Lat} {Sw.Lng}\n{Ne.Lat} {Ne.Lng}";
    }
}
