using System;

using AirlyAPI.Interactions;
using AirlyAPI.Rest;
using AirlyAPI.Utilities;

// todo przekodowac na ConfigureAwait(false) poniewaz jak zrobie synchroniczne to rzuca agregatem i robi sie deadlock (3x exception)

namespace AirlyAPI
{
    public class Airly : IDisposable
    {
        public string ApiKey { get; private set;  }
        protected internal RESTManager Rest { get; set; }

        public Installations Installations { get; private set; }
        public Measurements Measurements { get; private set; }
        public Meta Meta { get; private set; }

        public AirlyConfiguration Configuration { get; set; } = new AirlyConfiguration();
        public AirlyLanguage Language {
            get  {
                return this.Rest.Lang;
            }
            set {
                this.Rest.Lang = value;
            }
        }
        protected bool _isDisposed;

        // Indicates if the last request got ratelimited
        public bool IsRateLimited
        {
            get {
                return this.Rest.RateLimited;
            }
        }

        // Simply creating a new airly with the same configuration but with new rest
        public Airly(Airly airly)
        {
            this.ApiKey = airly.ApiKey;
            this.Language = airly.Language;
            this.Configuration = airly.Configuration;

            Initialize();
        }

        public Airly(Airly airly, AirlyConfiguration configuration)
        {
            this.ApiKey = airly.ApiKey;
            this.Language = airly.Language;
            this.Configuration = configuration;

            Initialize();
        }

        public Airly(string apiKey)
        {
           this.ApiKey = apiKey;
           
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

            Utils.ValidateKey(this.ApiKey);
        }

        public override string ToString() => ApiKey;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                this.Rest.Dispose();
                this.ApiKey = null;
                this._isDisposed = true;
            }
        }
    }
}
