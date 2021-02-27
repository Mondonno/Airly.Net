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
        public RESTManager rest { get; set; }
        public Airly airly { get; set; }

        public string domain
        {
            get => Utils.domain;
        }

        // api shortcut
        public async Task<T> api<T>(string end, dynamic query) => await rest.api<T>(end, query);

        dynamic endPoints = new
        {
            meta = "meta",
            installations = "installations",
            measurements = "measurements"
        };

        public InteractionBase(Airly airly)
        {
            this.airly = airly;
            rest = new RESTManager(airly);
        }
    }
}
