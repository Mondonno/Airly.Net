using System;
using AirlyNet.Common.Models;

namespace AirlyNet.Utilities
{
    public static class GeoUtil
    {
        public static double GetMidpoint(double pointOne, double pointTwo) => Math.Min(pointOne, pointTwo) + (Math.Abs(pointOne - pointTwo) / 2);

        public static bool Contains(double point, double begin, double end) => (point >= Math.Min(begin, end)) && (point <= Math.Max(begin, end));

        public static double GreatCircleDistanceInKm(AirLocation pointOne, AirLocation pointTwo)
        {
            bool simmilarCheck = (pointOne.Lat == pointTwo.Lat) && (pointOne.Lng == pointTwo.Lng);
            if (simmilarCheck) return 0.0;

            double oneLat = pointOne.Lat;
            double twoLat = pointTwo.Lat;

            double oneRadian = MathUtil.ToRadians(oneLat);
            double twoRadian = MathUtil.ToRadians(twoLat);

            double theta = pointOne.Lng - pointTwo.Lng;
            double dist = Math.Sin(oneRadian) *
                Math.Sin(twoRadian) +
                Math.Cos(oneRadian) *
                Math.Cos(twoRadian) *

                Math.Cos(MathUtil.ToRadians(theta));

            dist = Math.Acos(dist);
            dist = MathUtil.ToDegrees(dist);

            double finalDist = dist * 60.0 * 1.1515 * 1.609344;
            return finalDist;
        }

        public static double GetKmInArea(AirLocationArea area)
        {
            AirLocation sw = area.Sw;
            AirLocation ne = area.Ne;
            AirLocation barycenter = area.GetBarycenter();

            double km = Math.Max(
                GreatCircleDistanceInKm(sw, barycenter),
                GreatCircleDistanceInKm(ne, barycenter)
            );

            return km;
        }
    }

    public static class MathUtil
    {
        public static double ToRadians(double degrees) => degrees / 360 * (2 * Math.PI);

        public static double ToDegrees(double radians) => radians / (2 * Math.PI) * 360;
    }
}
