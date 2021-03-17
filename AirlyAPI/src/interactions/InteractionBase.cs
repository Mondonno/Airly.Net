using AirlyAPI.Rest;

namespace AirlyAPI.Interactions
{
    public class InteractionBase
    {
        private RESTManager Rest { get; set; }
        protected Airly Airly { get; private set; }
        protected RestApiClient Api { get; private set; }

        protected string Domain { get => Airly.Configuration.Domain; }

        public InteractionBase(Airly airly)
        {
            Airly = airly;
            Rest = airly.Rest;
            Api = new RestApiClient(Rest);
        }
    }
}
