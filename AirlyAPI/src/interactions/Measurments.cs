using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlyAPI.Interactions
{
    public class Measurements : InteractionBase
    {
        public Measurements(Airly airly) : base(airly) { }
        private string ResolveIndexType(IndexQueryType type) => type == IndexQueryType.AirlyCAQI ? "AIRLY_CAQI" : (type == IndexQueryType.CAQI ? "CAQI" : (type == IndexQueryType.PJP ? "PJP" : "AIRLY_CAQI"));

        public async Task<Measurement> Nearest(Location location, double maxDistance = 3, IndexQueryType type = IndexQueryType.AirlyCAQI) => await this.Nearest(location.Lat, location.Lng, maxDistance, type);
        public async Task<Measurement> Nearest(double lat, double lng, double maxDistance = 3, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/nearest", new
        {
            lat,
            lng,
            maxDistanceKM = maxDistance,
            indexType = ResolveIndexType(type)
        });

        public async Task<Measurement> Point(Location location, IndexQueryType type = IndexQueryType.AirlyCAQI) => await this.Point(location.Lat, location.Lng, type);
        public async Task<Measurement> Point(double lat, double lng, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/point", new {
            lat,
            lng,
            indexType = ResolveIndexType(type)
        });

        public async Task<Measurement> Installation(int id, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/installation", new {
            installationId = id,
            includeWind,
            indexType = ResolveIndexType(type)
        });

        // Non tested air code (new Airly API feature)
        public async Task<Measurement> Location(int locationId, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api<Measurement>("measurements/location", new
        {
            locationId,
            includeWind,
            type = ResolveIndexType(type)
        });
    }
}
