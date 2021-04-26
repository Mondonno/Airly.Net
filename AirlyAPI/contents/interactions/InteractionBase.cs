using AirlyNet.Rest;

namespace AirlyNet.Interactions
{
    public class InteractionBase<T>
    {

    }

    public class InteractionBase
    {
        protected Airly Airly { get; private set; }
        protected RestApiClient Api { get; private set; }

        protected string Domain { get => Airly.Configuration.Domain; }

        public InteractionBase(Airly airly)
        {
            Airly = airly;
            Api = airly.Client;
        }
    }
}
