using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AirlyNet.Utilities;
using AirlyNet.Common.Models;
using AirlyNet.Common;

namespace AirlyNet.Rest
{
    public class RestApiClientBase
    {
        private RESTManager RestManager { get; set; }
        protected RESTManager Api { get => RestManager; }
        protected RestApiClientBase(RESTManager rest) => RestManager = rest;

        protected string ResolveIndexType(AirlyIndexQueryType type) => type == AirlyIndexQueryType.AirlyCAQI ? "AIRLY_CAQI" : (type == AirlyIndexQueryType.CAQI ? "CAQI" : (type == AirlyIndexQueryType.PJP ? "PJP" : "AIRLY_CAQI"));
    }

    public class RestApiClient : RestApiClientBase
    {
        public RestApiClient(RESTManager rest) : base(rest: rest) { }

        public async Task<AirInstallation> GetInstallationByIdAsync(int id)
        {
            ParamsValidator.ThrowIfNegativeNumber(id);
            return await Api.Request<AirInstallation>($"installations/{id}", method: null, options: null);
        }

        public async Task<List<AirInstallation>> GetInstallationsNearestAsync(double lat, double lng, double maxDistance = 3, int maxResults = 1)
        {
            return await Api.Request<List<AirInstallation>>("installations/nearest", "GET", new
            {
                lat,
                lng,
                maxDistanceKM = ParamsValidator.InfinityToDouble(maxDistance),
                maxResults
            });
        }

        public async Task<AirMeasurement> GetMeasurementByInstallationAsync(int id, bool includeWind = false, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI)
        {
            ParamsValidator.ThrowIfNegativeNumber(id);
            return await Api.Request<AirMeasurement>("measurements/installation", "GET", new
            {
                installationId = id,
                includeWind,
                indexType = ResolveIndexType(type)
            });
        }

        public async Task<AirMeasurement> GetMeasurementNearestAsync(double lat, double lng, double maxDistance = 3, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI)
        {
            return await Api.Request<AirMeasurement>("measurements/nearest", "GET", new
            {
                lat,
                lng,
                maxDistanceKM = ParamsValidator.InfinityToDouble(maxDistance),
                indexType = ResolveIndexType(type)
            });
        }

        public async Task<AirMeasurement> GetMeasurementByPointAsync(double lat, double lng, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI)
        {
            return await Api.Request<AirMeasurement>("measurements/point", "GET", new
            {
                lat,
                lng,
                indexType = ResolveIndexType(type)
            });
        }

        public async Task<AirMeasurement> GetMeasurementByLocationIdAsync(int locationId, bool includeWind = false, AirlyIndexQueryType type = AirlyIndexQueryType.AirlyCAQI)
        {
            ParamsValidator.ThrowIfNegativeNumber(locationId);
            return await Api.Request<AirMeasurement>(end: "measurements/location", "GET", new
            {
                locationId,
                includeWind,
                type = ResolveIndexType(type)
            });
        }

        public async Task<List<AirIndexType>> GetMetaIndexesAsync() => await Api.Request<List<AirIndexType>>(end: "meta/indexes", method: null, options: null);

        public async Task<List<AirMeasurementType>> GetMetaMeasurmentsAsync() => await Api.Request<List<AirMeasurementType>>(end: "meta/measurements", method: null, options: null);

        // working on the OpenAPI endpoint https://airapi.airly.eu/docs/v{versionCode}
        public Task<object> GetAirlyOpenDocsAsync() => null;
    }
}
