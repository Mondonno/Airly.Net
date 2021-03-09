using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlyAPI.Interactions
{
    public class Meta : InteractionBase
    {
        public Meta(Airly airly) : base(airly) { }

        public async Task<List<IndexType>> Indexes() => await Api<List<IndexType>>("meta/indexes", new { });
        public async Task<List<MeasurementType>> Measurements() => await Api<List<MeasurementType>>("meta/measurements", new { });

        //        working on the OpenAPI endpoint
        // >> https://airapi.airly.eu/docs/v{versionCode}
        public Task<object> Docs() => null;
    }
}
