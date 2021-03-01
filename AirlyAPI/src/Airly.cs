//
// AIRLY CLIENT 
//
// Dependecies of Airly API
// -- Newtonsoft.Json
// -- Lastest C# & LINQ

using AirlyAPI.Interactions;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

// Todo fix the language setting
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
                Debug.WriteLine($"Lang setter:    {value}"); // todo delete this because this is only for the debug
                this.Rest.Lang = value;
            }
        }

        public Airly(string apiKey)
        {
           this.ApiKey = apiKey;
           this.Configuration = new AirlyConfiguration();
            
           Init(true);
        }

        public Airly(string apiKey, AirlyConfiguration configuration)
        {
            this.ApiKey = apiKey;
            this.Configuration = configuration;

            Init(true);
        }

        private void Init(bool lang)
        {
            Rest = new RESTManager(this);

            Installations = new Installations(this, this.Rest);
            Meta = new Meta(this, this.Rest);
            Measurments = new Measurments(this, this.Rest);

            _ = lang ? Language = AirlyLanguage.en : Language;

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
