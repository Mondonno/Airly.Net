using System;
using System.Threading.Tasks;

using AirlyNet.Models;

namespace AirlyNet.Interactions
{
    public class Measurements : InteractionBase
    {
        public Measurements(Airly airly) : base(airly) { }

        /// <summary>
        /// Getting the nearest measurment's of the installation location.
        /// </summary>
        /// <param name="installation">Installation object</param>
        /// <param name="maxDistance">Max distance, deafult to 3, double</param>
        /// <param name="type">Type of the measurment index</param>
        /// <returns></returns>
        public async Task<AirMeasurement> Nearest(AirInstallation installation, double maxDistance = 3, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Nearest(installation.Location, maxDistance, type);
        /// <summary>
        /// Getting the nearest measurment's of the specified location.
        /// </summary>
        /// <param name="location">Location object</param>
        /// <param name="maxDistance">Max distance, deafult to 3, double</param>
        /// <param name="type">Type of the measurment index</param>
        /// <returns></returns>
        public async Task<AirMeasurement> Nearest(Location location, double maxDistance = 3, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Nearest(location.Lat, location.Lng, maxDistance, type);

        private async Task<AirMeasurement> Nearest(double lat, double lng, double maxDistance = 3, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Api.GetMeasurmentNearestAsync(lat, lng, maxDistance, type);

        /// <summary>
        /// Use Point when you need to get the measurment from the point. Else use Nearest to get the measurment nearest the point.
        /// If no measurment found, this endpoint will return null
        /// </summary>
        /// <param name="installation">Installation object</param>
        /// <param name="type">Type of the measurment Index</param>
        /// <returns>Single measurment</returns>
        public async Task<AirMeasurement> Point(AirInstallation installation, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Point(installation.Location, type);

        /// <summary>
        /// Getting the Point() but of the geografic Location.
        /// </summary>
        /// <param name="location">Location object</param>
        /// <param name="type">Index of the measurment Index</param>
        /// <returns>Single measurment</returns>
        public async Task<AirMeasurement> Point(Location location, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Point(location.Lat, location.Lng, type);

        private async Task<AirMeasurement> Point(double lat, double lng, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Api.GetMeasurmentByPointAsync(lat, lng, type);

        /// <summary>
        /// Use this when you need to get the informations about installation by ID
        /// </summary>
        /// <param name="id">ID of installation</param>
        /// <param name="includeWind">Check this if you want to include the wind measurments</param>
        /// <param name="type">Type of the measurment Index</param>
        /// <returns>Single measurment</returns>
        public async Task<AirMeasurement> Installation(int id, bool includeWind = false, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Api.GetMeasurmentByInstallationAsync(id, includeWind, type);

        /// <summary>
        /// Getting the installation pinned measurment.
        /// </summary>
        /// <param name="installation">Installation object</param>
        /// <param name="includeWind">Check this if you want to include the wind measurments</param>
        /// <param name="type">Type of the measurment index</param>
        /// <returns>Single measurment</returns>
        public async Task<AirMeasurement> Installation(AirInstallation installation, bool includeWind = false, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => installation != null ? await Installation(installation.Id, includeWind, type) : null;

        /// <summary>
        /// Use this to get the measurment's by the locationId.
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="includeWind">Check this if you want to include the wind measurments</param>
        /// <param name="type">Type of the measurment Index</param>
        /// <returns>Single measurment</returns>
        public async Task<AirMeasurement> Location(int locationId, bool includeWind = false, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) => await Api.GetMeasurmentByLocationIdAsync(locationId, includeWind, type);

        /// <summary>
        /// Getting the measurment by the location id from installation. Remember that the location id is nullable property
        /// </summary>
        /// <param name="installation">Installation object</param>
        /// <param name="includeWind">Check this if you want to include the wind measurments</param>
        /// <param name="type">Type of the measurment Index</param>
        /// <returns>Single measurment</returns>
        public async Task<AirMeasurement> Location(AirInstallation installation, bool includeWind = false, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI) =>
            installation == null || installation.LocationId == null
                ? null
                : await Location((int) installation.LocationId, includeWind, type);
    }
}
