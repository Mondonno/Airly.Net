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

        public Installations installations { get; set; }
        public Measurments measurments { get; set; }
        public Meta meta { get; set; }

        public AirlyLanguage language { get => language; set => rest.lang = value; }

        private void Init()
        {
            language = AirlyLanguage.en;

            rest = new RESTManager(this);
            installations = new Installations(this);
            meta = new Meta(this);
            measurments = new Measurments(this);
        }

        public Airly(string apiKey)
        {
           this.apiKey = apiKey;
           Init();
        }
    }
}
