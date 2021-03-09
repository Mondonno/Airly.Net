using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using AirlyAPI.Handling;
using AirlyAPI.Utilities;

namespace AirlyAPI.Rest
{
    // The routes is the wrapper for the Request Module with the prered useful methods
    // The reuqest manager is the manager of all API operations
    // :)
    public class RESTManager : BasicRoutes, IDisposable
    {
        private string ApiKey { get; set; }
        private RequestQueueHandler Handlers { get; set; }

        public Airly Airly { get; set; }
        public AirlyLanguage Lang { get; set; }

        public RESTManager(Airly airly, string apiKey)
        {
            this.ApiKey = apiKey;
            this.Airly = airly;
            this.Handlers = new RequestQueueHandler(this);
        }

        public RESTManager(Airly airly) : this(airly, airly.ApiKey) { }

        public string Auth
        {
            get
            {
                AirlyError err = new AirlyError("Provided api key is empty");
                err.Data.Add("Token", false);

                if (string.IsNullOrEmpty(ApiKey)) throw err;
                return this.ApiKey;
            }
        }

        public bool RateLimited
        {
            get
            {
                if (this.Handlers.Queuers() >= 1) return this.Handlers.RateLimited();
                else return false;
            }
        }

        public string Endpoint { get => this.Airly.Configuration.ApiDomain; }
        public string Cdn { get => this.Airly.Configuration.Cdn; }

        private void SetAirlyPreferedLanguage(Airly air) => this.Lang = air.Language;

        private object ValidateLang(object lang)
        {
            if (lang == null) return AirlyLanguage.en;

            // Declaring the namespace and the type of the checking object
            string air = string.Format("{0}.{1}", nameof(AirlyAPI), nameof(AirlyLanguage));
            string matchType = lang.GetType().ToString();

            if (matchType == air) return lang;
            else return AirlyLanguage.en;
        }

        public Task<AirlyResponse> Request(string end, string method, RequestOptions options = null)
        {
            if (options == null) options = new RequestOptions();

            if(this.ApiKey != null) options.Auth = true;

            string route = Utils.GetRoute(end);
            DeafultRestRequest Request = new DeafultRestRequest(this, end, method, options);

            Request.SetKey(this.ApiKey);
            Request.SetLanguage(this.Lang);

            return Handlers.Queue(route, Request);
        }
        public async Task<T> RequestAndParseType<T>(string end, string method, dynamic query, bool versioned)
        {
            Utils util = new Utils();
            RequestOptions options = new RequestOptions()
            {
                Query = util.ParseQuery(query),
                Versioned = versioned
            };
            var httpResponse = await Request(end, method ?? "GET", options);
            return new RestResponseParser<T>(httpResponse.rawJSON).Deserializated;
        }

        // Simple get wrapper (because only GET requests Airly API accepts)
        public async Task<T> Api<T>(string end, dynamic query) where T : class => await RequestAndParseType<T>(end, "GET", query, true);
        public async Task<T> Api<T>(string end, dynamic query, bool versioned) where T : class => await RequestAndParseType<T>(end, "GET", query, versioned);

        public void Dispose() => this.Handlers.Reset();
    }

    public interface IBaseRouter { }

    public class BasicRoutes : IBaseRouter
    {
        public BasicRoutes(){}

        public override string ToString() => base.ToString();
    }
}
