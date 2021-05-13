using System;
using System.Threading.Tasks;

namespace AirlyNet.Interactions
{
    public class Measurements : InteractionBase
    {
        public Measurements(Airly airly) : base(airly) { }

        public async Task<Measurement> Nearest(Installation installation, double maxDistance = 3, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Nearest(installation.Location, maxDistance, type);
        public async Task<Measurement> Nearest(Location location, double maxDistance = 3, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Nearest(location.Lat, location.Lng, maxDistance, type);
        private async Task<Measurement> Nearest(double lat, double lng, double maxDistance = 3, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api.GetMeasurmentNearestAsync(lat, lng, maxDistance, type);

        public async Task<Measurement> Point(Installation installation, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Point(installation.Location, type);
        public async Task<Measurement> Point(Location location, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Point(location.Lat, location.Lng, type);
        private async Task<Measurement> Point(double lat, double lng, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api.GetMeasurmentByPointAsync(lat, lng, type);

        public async Task<Measurement> Installation(int id, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api.GetMeasurmentByInstallationAsync(id, includeWind, type);
        public async Task<Measurement> Installation(Installation installation, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI) => installation != null ? await Installation(installation.Id, includeWind, type) : null;

        public async Task<Measurement> Location(int locationId, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI) => await Api.GetMeasurmentByLocationIdAsync(locationId, includeWind, type);
        public async Task<Measurement> Location(Installation installation, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI) =>
            installation == null || installation.LocationId == null
                ? null
                : await Location((int) installation.LocationId, includeWind, type);
    }
}
