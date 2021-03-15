using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlyAPI.Interactions
{
    public class Meta : InteractionBase
    {
        public Meta(Airly airly) : base(airly) { }
        public async Task<List<IndexType>> Indexes() => await Api.GetMetaIndexesAsync();
        public async Task<List<MeasurementType>> Measurements() => await Api.GetMetaMeasurmentsAsync();

        public async Task<object> Docs() => await Api.GetAirlyOpenDocsAsync();
    }
}
