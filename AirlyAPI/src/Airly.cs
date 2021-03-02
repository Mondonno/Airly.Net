//
// AIRLY CLIENT 
//
// Dependecies of Airly API
// -- Newtonsoft.Json
// -- Lastest C# (preffered 8.0+) & LINQ

using AirlyAPI.Interactions;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

// Todo dodac do Airly metode api publiczna wrapowana z Rest (po to zeby interfejsy API mialy od razu api connect bez przekazywania RESTManager)
// Todo please look to Managers.cs for todo :) (IMPORTANT !!!)

namespace AirlyAPI
{
    public class Airly
    {
        public string ApiKey { get; }
        private RESTManager Rest { get; set; }

        public Installations Installations { get; private set; }
        public Measurments Measurments { get; private set; }
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

            Installations = new Installations(this, this.Rest);
            Meta = new Meta(this, this.Rest);
            Measurments = new Measurments(this, this.Rest);

            Language = AirlyLanguage.en;

            KeyCheck();
        }

        private void KeyCheck()
        {
            AirlyError invalidToken = new AirlyError("Provided airly api key is invalid");
            invalidToken.Data.Add("Token", false);

            string validatedKey = ApiKey.Replace(" ", "").Trim().Normalize();
            if (string.IsNullOrEmpty(validatedKey) || validatedKey != ApiKey) throw invalidToken;
            else return;
        }
    }
}
