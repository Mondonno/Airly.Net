using System;
using System.Threading.Tasks;

using AirlyAPI.Rest;

namespace AirlyAPI.Interactions
{
    public interface IBaseInterface { }

    public class InteractionBase : IBaseInterface
    {
        private RESTManager Rest { get; set; }

        protected Airly Airly { get; set; }
        protected string Domain { get => this.Airly.Configuration.Domain; }

        public InteractionBase(Airly airly)
        {
            this.Airly = airly;
            this.Rest = airly.Rest;
        }

        // api shortcut
        protected async Task<T> Api<T>(string end, dynamic query) => await Rest.Api<T>(end, query);
    }
}
