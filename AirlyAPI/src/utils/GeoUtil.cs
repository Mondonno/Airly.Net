using System;

namespace AirlyAPI.Utilities
{
    public static class GeoUtil
    {
        private static double ToRadians(double degrees) => degrees / 360 * (2 * Math.PI);
        private static double ToDegrees(double radians) => radians / (2 * Math.PI) * 360;

        public static double GetMidpoint(double pointOne, double pointTwo) => Math.Min(pointOne, pointTwo) + (Math.Abs(pointOne - pointTwo) / 2);
        public static bool Contains(double point, double begin, double end) => (point >= Math.Min(begin, end)) && (point <= Math.Max(begin, end));
        public static double GreatCircleDistanceInKm(Location pointOne, Location pointTwo)
        {
            bool simmilarCheck = (pointOne.Lat == pointTwo.Lat) && (pointOne.Lng == pointTwo.Lng);
            if (simmilarCheck) return 0.0;

            double oneLat = pointOne.Lat;
            double twoLat = pointTwo.Lat;

            double oneRadian = ToRadians(oneLat);
            double twoRadian = ToRadians(twoLat);

            double theta = pointOne.Lng - pointTwo.Lng;
            double dist = Math.Sin(oneRadian) *
                Math.Sin(twoRadian) +
                Math.Cos(oneRadian) *
                Math.Cos(twoRadian) *

                Math.Cos(ToRadians(theta));

            dist = Math.Acos(dist);
            dist = ToDegrees(dist);

            double finalDist = dist * 60.0 * 1.1515 * 1.609344;
            return finalDist;
        }

        public static double GetKmInArea(LocationArea area)
        {
            Location sw = area.Sw;
            Location ne = area.Ne;
            Location barycenter = area.GetBarycenter();

            double km = Math.Max(
                GreatCircleDistanceInKm(sw, barycenter),
                GreatCircleDistanceInKm(ne, barycenter)
            );

            return km;
        }
    }
}
