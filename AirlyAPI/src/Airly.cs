//
// AIRLY CLIENT 
//
// Dependecies of Airly API
// -- Newtonsoft.Json
// -- Lastest C# & LINQ

using System.Threading.Tasks;
using System;
using AirlyAPI.Interactions;

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

        private void Init()
        {
            rest = new RESTManager(this);

            Installations = new Installations(this);
            Meta = new Meta(this);
            Measurments = new Measurments(this);

            language = AirlyLanguage.en;
        }

        public Airly(string apiKey)
        {
           this.apiKey = apiKey;
           Init();
        }
    }
}
