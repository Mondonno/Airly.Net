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
        public string language { get; set; }

        private RESTManager rest { get; set; }

        public Installations installations { get; }
        public Measurments measurments { get; }
        public Meta meta { get; }

        private void InitProperties()
        {

        }

        public Airly(string apiKey)
        {
           this.apiKey = apiKey;
           InitProperties(); // Initializing properties on client (eg. creating new classes instances)
        }
    }
}
