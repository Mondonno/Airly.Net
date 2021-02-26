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

        public Installations Installations { get; set; }
        public Measurments Measurments { get; set; }
        public Meta Meta { get; set; }

        public AirlyConfiguration Configuration { get; set; }
        public AirlyLanguage language {
            get  {
                return rest.lang;
            }
            set {
                setLanguage(value);
            }
        }

        private void setLanguage(AirlyLanguage lang) => rest.lang = lang;

        private void Init(bool lang)
        {
            rest = new RESTManager(this);

            Installations = new Installations(this);
            Meta = new Meta(this);
            Measurments = new Measurments(this);

            _ = lang ? language = AirlyLanguage.en : AirlyLanguage.en; 
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
