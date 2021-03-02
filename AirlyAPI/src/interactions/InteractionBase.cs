using System;
using System.Threading.Tasks;

namespace AirlyAPI.Interactions
{
    public interface BaseInterface
    {
        string domain { get; }
    }
    public class InteractionBase : BaseInterface
    {
        private RESTManager Rest { get; set; }
        public Airly Airly { get; set; }

        public InteractionBase(Airly airly, RESTManager rest)
        {
            this.Airly = airly;
            this.Rest = rest;
        }

        public string domain
        {
            get => Utils.domain;
        }

        // api shortcut
        public async Task<T> api<T>(string end, dynamic query) => await Rest.api<T>(end, query);

        dynamic endPoints = new
        {
            meta = "meta",
            installations = "installations",
            measurements = "measurements"
        };
    }
}
