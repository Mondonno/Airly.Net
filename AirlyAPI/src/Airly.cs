//
// AIRLY CLIENT 
//
// Dependecies of Airly API
// -- Newtonsoft.Json
// -- Lastest C# & LINQ
 
using System.Threading.Tasks;
using System;
using AirlyAPI.Interactions;

// Todo please look to Managers.cs for todo :) (IMPORTANT !!!)

namespace AirlyAPI
{
    public class Airly
    {
        public string apiKey { get; }
        private RESTManager rest { get; set; }

        public Installations Installations { get; private set; }
        public Measurments Measurments { get; private set; }
        public Meta Meta { get; private set; }

        public AirlyConfiguration Configuration { get; set; }
        public AirlyLanguage language {
            get  {
                return rest.lang;
            }
            set {
                rest.lang = value;
            }
        }

        // Initializing all required classes and properties
        private void Init(bool lang)
        {
            rest = new RESTManager(this);

            Installations = new Installations(this);
            Meta = new Meta(this);
            Measurments = new Measurments(this);

            _ = lang ? language = AirlyLanguage.en : language; 
        }

        public Airly(string apiKey)
        {
           this.apiKey = apiKey;
           Init(true);
        }

        public Airly(string apiKey, AirlyLanguage language)
        {
            this.apiKey = apiKey;
            Init(false);

            this.language = language;
        }
    }
}
