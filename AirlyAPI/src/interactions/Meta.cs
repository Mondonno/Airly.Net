using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlyAPI.Interactions
{
    public class Meta : InteractionBase
    {
        public async Task<List<IndexType>> Indexes() => await api<List<IndexType>>("meta/indexes", new { });
        public async Task<List<MeasurementType>> Measurements() => await api<List<MeasurementType>>("meta/measurements", new { });
    }
}
 