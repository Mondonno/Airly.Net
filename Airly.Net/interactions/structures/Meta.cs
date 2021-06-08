using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using AirlyNet.Models;

namespace AirlyNet.Interactions
{
    public class Meta : InteractionBase
    {
        public Meta(Airly airly) : base(airly) { }

        /// <summary>
        /// Getting the measurments indexes, for example AIRLY_CAQI.
        /// </summary>
        /// <returns>List of the Index'es (IndexType)</returns>
        public async Task<List<IndexType>> Indexes() => await Api.GetMetaIndexesAsync();

        /// <summary>
        /// Getting the meta of the measurments limits, eg. WHO recommendations 
        /// </summary>
        /// <returns>List of the Measurment's Meta (MeasurmentType)</returns>
        public async Task<List<MeasurementType>> Measurements() => await Api.GetMetaMeasurmentsAsync();
        public Task<object> Docs() => throw new NotImplementedException();
    }
}
