using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using AirlyAPI.Handling;
using AirlyAPI.Utilities;

namespace AirlyAPI.Rest
{
    public interface IBaseRouter { }

    // The routes is the wrapper for the Request Module with the prered useful methods
    // The reuqest manager is the manager of all API operations
    // :)
    public class RESTManager : BasicRoutes
    {
        private string ApiKey { get; set; }
        private RequestQueueHandler Handlers { get; set; } = new RequestQueueHandler();

        public Airly Airly { get; set; }
        public AirlyLanguage Lang { get; set; }

        public RESTManager(Airly airly, string apiKey)
        {
            this.ApiKey = apiKey;
            this.Airly = airly;
        }

        public RESTManager(Airly airly)
        {
            this.Airly = airly;
            this.ApiKey = airly.ApiKey;
        }

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

        public string Endpoint { get => this.Airly.Configuration.ApiDomain; }
        public string Cdn { get => this.Airly.Configuration.Cdn; }

        private void SetAirlyPreferedLang(Airly air) => this.Lang = air.Language;

        private object ValidateLang(object lang)
        {
            if (lang == null) return AirlyLanguage.en;

            // Declaring the namespace and the type of the checking object
            string air = string.Format("{0}.{1}", nameof(AirlyAPI), nameof(AirlyLanguage));
            string matchType = lang.GetType().ToString();

            if (matchType == air) return lang;
            else return AirlyLanguage.en;
        }

        // Making the request to the API
        // Something like "core" wrapper
        public Task<AirlyResponse> Request(string end, string method, RequestOptions options = null)
        {
            if (options == null) options = new RequestOptions();

            options.auth = true;

            string route = Utils.GetRoute(end);
            var Request = new RequestModule(this, end, method, options);

            Request.SetKey(this.ApiKey);
            Request.SetLanguage(this.Lang);

            RequestQueuer handler = Handlers.Get(route);

            if(handler == null)
            {
                handler = new RequestQueuer(this);
                this.Handlers.Set(route, handler);
            }

            return handler.Push(Request);
        }

        // Simple get wrapper (because only GET requests Airly API accepts)
        public async Task<T> Api<T>(string end, dynamic query) {
            // Utils.ParseToClassJSON<T>((await Request(end, "get", new RequestOptions(new Utils().ParseQuery(query)))).JSON);

            var requestResponse = (await Request(end, "get", new RequestOptions(new Utils().ParseQuery(query))));
            JToken requestJsonResult ;

            if (requestResponse == null) return default;
            else requestJsonResult = requestResponse.JSON;

            var resultValue = JsonParser.ParseToClassJSON<T>(requestJsonResult);

            return resultValue;
        }
    }

    public class BasicRoutes : IBaseRouter
    {
        public BasicRoutes(){}

        public override string ToString() => base.ToString();
    }
}
