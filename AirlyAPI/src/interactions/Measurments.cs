using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlyAPI.Interactions
{
    public class Measurments : InteractionBase
    {
        public Measurments(Airly airly) : base(airly) { }

        public enum IndexType
        {
            AirlyCAQI,
            CAQI,
            PJP
        }

        // Added support for the Index Type parameter
        private string resolveIndexType(IndexType type) => type == IndexType.AirlyCAQI ? "AIRLY_CAQI" : (type == IndexType.CAQI ? "CAQI" : (type == IndexType.PJP ? "PJP" : "AIRLY_CAQI"));

        public async Task<List<Measurement>> Nearest(double lat, double lng, double maxDistance = 3, IndexType type = IndexType.AirlyCAQI) => await api<List<Measurement>>("measurements/nearest", new
        {
            lat,
            lng,
            maxDistanceKM = maxDistance,
            indexType = resolveIndexType(type)
        });

        public async Task<Measurement> Point(double lat, double lng, IndexType type = IndexType.AirlyCAQI) => await api<Measurement>("measurements/point", new { lat, lng, indexType = resolveIndexType(type) });

        // Se what is the redirect in the Installations.cs file
        public async Task<Measurement> Installation(int installationId, bool redirect = false, IndexType type = IndexType.AirlyCAQI) => await api<Measurement>("measurements/installation", new { installationId, indexType = resolveIndexType(type) });

        //public async Task<Measurement> Area(LocationArea area) =>
    }
}
