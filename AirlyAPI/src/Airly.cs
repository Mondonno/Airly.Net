//
// AIRLY CLIENT 
//
// Dependecies of Airly API
// -- Newtonsoft.Json
// -- Lastest C# (preffered 8.0+) & LINQ

using AirlyAPI.Interactions;
using AirlyAPI.Rest;

// Todo dodac do Airly metode api publiczna wrapowana z Rest (po to zeby interfejsy API mialy od razu api connect bez przekazywania RESTManager)
// Todo please look to Managers.cs for todo :) (IMPORTANT !!!)

namespace AirlyAPI
{
    public class Airly
    {
        public string ApiKey { get; }
        internal RESTManager Rest { get; set; }

        public Installations Installations { get; private set; }
        public Measurements Measurements { get; private set; }
        public Meta Meta { get; private set; }

        public AirlyConfiguration Configuration { get; set; }
        public AirlyLanguage Language {
            get  {
                return this.Rest.Lang;
            }
            set {
                this.Rest.Lang = value;
            }
        }

        // Simply creating a new airly with the same configuration but with new rest
        public Airly(Airly airly)
        {
            this.ApiKey = airly.ApiKey;
            this.Configuration = airly.Configuration;
            this.Language = airly.Language;

            Initialize();
        }

        public Airly(Airly airly, AirlyConfiguration configuration)
        {
            this.Configuration = configuration;
            this.ApiKey = airly.ApiKey;
            this.Language = airly.Language;

            Initialize();
        }

        public Airly(string apiKey)
        {
           this.ApiKey = apiKey;
           this.Configuration = new AirlyConfiguration();
            
           Initialize();
        }

        public Airly(string apiKey, AirlyConfiguration configuration)
        {
            this.ApiKey = apiKey;
            this.Configuration = configuration;

            Initialize();
        }

        private void Initialize()
        {
            Rest = new RESTManager(this);

            Installations = new Installations(this);
            Meta = new Meta(this);
            Measurements = new Measurements(this);

            KeyCheck();
        }

        private void KeyCheck()
        {
            // Simple airly key validation (eg. key of whitespaces)
            string toValidate = this.ApiKey;

            AirlyError invalidToken = new AirlyError("Provided airly api key is invalid");
            invalidToken.Data.Add("Token", false);

            string validatedKey = toValidate.Replace(" ", "").Trim().Normalize();

            if (string.IsNullOrEmpty(toValidate) || string.IsNullOrEmpty(validatedKey) || validatedKey != toValidate) throw invalidToken;
            else return;
        }
    }
}
