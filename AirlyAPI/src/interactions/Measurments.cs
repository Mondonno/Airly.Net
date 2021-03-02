using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlyAPI.Interactions
{
    public class Measurments : InteractionBase
    {
        public Measurments(Airly airly, RESTManager rest) : base(airly, rest) { }

        // Added support for the Index Type parameter
        private string resolveIndexType(IndexQueryType type) => type == IndexQueryType.AirlyCAQI ? "AIRLY_CAQI" : (type == IndexQueryType.CAQI ? "CAQI" : (type == IndexQueryType.PJP ? "PJP" : "AIRLY_CAQI"));

        public async Task<List<Measurement>> Nearest(double lat, double lng, double maxDistance = 3, IndexQueryType type = IndexQueryType.AirlyCAQI) => await api<List<Measurement>>("measurements/nearest", new
        {
            lat,
            lng,
            maxDistanceKM = maxDistance,
            indexType = resolveIndexType(type)
        });

        public async Task<Measurement> Point(double lat, double lng, IndexQueryType type = IndexQueryType.AirlyCAQI) => await api<Measurement>("measurements/point", new {
            lat,
            lng,
            indexType = resolveIndexType(type)
        });

        public async Task<Measurement> Installation(int id, IndexQueryType type = IndexQueryType.AirlyCAQI, bool redirect = false) => await api<Measurement>("measurements/installation", new {
            installationId = id,
            indexType = resolveIndexType(type)
        });
    }
}
