using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
        public AirlyLanguage Lang { get; set; }

        public RESTManager(Airly airly, string apiKey)
        {
            this.apiKey = apiKey;
            this.airly = airly;
        }

        public RESTManager(Airly airly)
        {
            this.airly = airly;
            this.apiKey = airly.ApiKey;
        }

        public string Auth
        {
            get
            {
                AirlyError err = new AirlyError("Provided api key is empty");
                err.Data.Add("Token", false);

                if (string.IsNullOrEmpty(apiKey)) throw err;
                return this.apiKey;
            }
        }

        public string Endpoint { get => $"{this.airly.Configuration.ApiDomain}"; }
        public string Cdn { get => $"cdn.{Utils.domain}"; }

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
        public async Task<AirlyResponse> Request(string end, string method, RequestOptions options = null)
        {
            var util = new Utils();
            if (options == null) options = new RequestOptions(new string[0][]);

            RequestModule requestManager = new RequestModule(end, method.ToUpper(), options);

            requestManager.setKey(apiKey);
            requestManager.SetLanguage(this.Lang);

            var response = await requestManager.MakeRequest();
            string dateHeader = util.getHeader(response.headers, "Date");

            DateTime date = DateTime.Parse(dateHeader ?? DateTime.Now.ToString()); // If the date header is null setting the date for actual date
            return new AirlyResponse(response, date);
        }

        // Simple get wrapper (because only GET requests Airly API accepts)
        public async Task<T> api<T>(string end, dynamic query) {
            // Utils.ParseToClassJSON<T>((await Request(end, "get", new RequestOptions(new Utils().ParseQuery(query)))).JSON);

            var requestJsonResult = (await Request(end, "get", new RequestOptions(new Utils().ParseQuery(query)))).JSON;
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
