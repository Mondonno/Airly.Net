using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AirlyAPI
{
    public interface IBaseRouter { }

    // The routes is the wrapper for the Request Module with the prered useful methods
    // The reuqest manager is the manager of all API operations
    // :)
    public class RESTManager : BasicRoutes
    {
        private Airly airly { get; set; }
        private string apiKey { get; set; }
        public AirlyLanguage lang { get; set; } = AirlyLanguage.en;

        public RESTManager(Airly airly, string apiKey)
        {
            this.apiKey = apiKey;
            this.airly = airly;
        }

        public RESTManager(Airly airly)
        {
            this.airly = airly;
            this.apiKey = airly.apiKey;
        }

        public string Auth {
            get {
                if (string.IsNullOrEmpty(apiKey)) throw new AirlyError("Provided api key is empty");
                return this.apiKey;
            }
        }

        public string Endpoint { get; set; }
        public string Cdn { get => $"cdn.{Utils.domain}"; }

        private object ValidateLang(object lang)
        {
            string air = nameof(AirlyLanguage);
            if (lang.GetType().ToString() == air) return lang;
            else return AirlyLanguage.en;
        }

        // Making the request to the API
        // Something like "core" wrapper
        public async Task<AirlyResponse> Request(string end, string method, RequestOptions options = null)
        {
            var util = new Utils();
            if (options == null) options = new RequestOptions(new string[0][]);

            var requestManager = new RequestModule(end, method, options);

            object lang = this.lang;
            requestManager.setKey(apiKey);
            requestManager.setLanguage((AirlyLanguage)ValidateLang(lang));

            var response = await requestManager.MakeRequest();
            string dateHeader = util.getHeader(response.headers, "Date");

            DateTime date = DateTime.Parse(dateHeader ?? DateTime.Now.ToString()); // If the date header is null setting the date for actual date
            return new AirlyResponse(response, date);
        }

        // Simple get wrapper (because only GET requests Airly API accepts)
        public async Task<T> api<T>(string end, dynamic query) => Utils.ParseToClassJSON<T>((await Request(end, "get", new RequestOptions(new Utils().ParseQuery(query)))).JSON);
    }

    public class BasicRoutes : IBaseRouter
    {
        public BasicRoutes(){}

        public override string ToString() => base.ToString();
    }
}
