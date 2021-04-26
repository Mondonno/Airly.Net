using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AirlyNet.Utilities;

namespace AirlyNet.Rest
{
    public class RestApiClientBase
    {
        private RESTManager RestManager { get; set; }
        protected RESTManager Api { get => RestManager; }
        protected RestApiClientBase(RESTManager rest) => RestManager = rest;

        protected string ResolveIndexType(IndexQueryType type) => type == IndexQueryType.AirlyCAQI ? "AIRLY_CAQI" : (type == IndexQueryType.CAQI ? "CAQI" : (type == IndexQueryType.PJP ? "PJP" : "AIRLY_CAQI"));
    }

    public class RestApiClient : RestApiClientBase
    {
        public RestApiClient(RESTManager rest) : base(rest: rest) { }
        public async Task<Installation> GetInstallationByIdAsync(int id)
        {
            ParamsValidator.ThrowIfNegativeNumber(id);
            return await Api.Request<Installation>($"installations/{id}", method: null, options: null);
        }
        public async Task<List<Installation>> GetInstallationsNearestAsync(double lat, double lng, double maxDistance = 3, int maxResults = 1)
        {
            return await Api.Request<List<Installation>>("installations/nearest", "GET", new
            {
                lat,
                lng,
                maxDistanceKM = ParamsValidator.InfinityToDouble(maxDistance),
                maxResults
            });
        }
        public async Task<Measurement> GetMeasurmentByInstallationAsync(int id, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI)
        {
            ParamsValidator.ThrowIfNegativeNumber(id);
            return await Api.Request<Measurement>("measurements/installation", "GET", new
            {
                installationId = id,
                includeWind,
                indexType = ResolveIndexType(type)
            });
        }
        public async Task<Measurement> GetMeasurmentNearestAsync(double lat, double lng, double maxDistance = 3, IndexQueryType type = IndexQueryType.AirlyCAQI)
        {
            return await Api.Request<Measurement>("measurements/nearest", "GET", new
            {
                lat,
                lng,
                maxDistanceKM = ParamsValidator.InfinityToDouble(maxDistance),
                indexType = ResolveIndexType(type)
            });
        }
        public async Task<Measurement> GetMeasurmentByPointAsync(double lat, double lng, IndexQueryType type = IndexQueryType.AirlyCAQI)
        {
            return await Api.Request<Measurement>("measurements/point", "GET", new
            {
                lat,
                lng,
                indexType = ResolveIndexType(type)
            });
        }
        public async Task<Measurement> GetMeasurmentByLocationIdAsync(int locationId, bool includeWind = false, IndexQueryType type = IndexQueryType.AirlyCAQI)
        {
            ParamsValidator.ThrowIfNegativeNumber(locationId);
            return await Api.Request<Measurement>(end: "measurements/location", "GET", new
            {
                locationId,
                includeWind,
                type = ResolveIndexType(type)
            });
        }
        public async Task<List<IndexType>> GetMetaIndexesAsync() => await Api.Request<List<IndexType>>(end: "meta/indexes", method: null, options: null);
        public async Task<List<MeasurementType>> GetMetaMeasurmentsAsync() => await Api.Request<List<MeasurementType>>(end: "meta/measurements", method: null, options: null);

        // working on the OpenAPI endpoint https://airapi.airly.eu/docs/v{versionCode}
        public Task<object> GetAirlyOpenDocsAsync() => null;
    }
}
