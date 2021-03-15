using System;

using AirlyAPI.Interactions;
using AirlyAPI.Rest;
using AirlyAPI.Utilities;

namespace AirlyAPI
{
    /// <summary>
    /// The entry point of the Airly API C# client. Providing the methods wich helps programmer interact with them
    /// </summary>
    public class Airly : IDisposable
    {
        protected bool _isDisposed;

        public string ApiKey { get; private set; }
        protected internal RESTManager Rest { get; set; }

        public Installations Installations { get; private set; }
        public Measurements Measurements { get; private set; }
        public Meta Meta { get; private set; }

        public AirlyConfiguration Configuration { get; set; } = new AirlyConfiguration();
        public AirlyLanguage Language {
            get => Rest.Language;
            set => Rest.Language = value;
        }
        public bool IsRateLimited { get => Rest.RateLimited; }

        private Airly(string apiKey, AirlyLanguage? language)
        {
            ApiKey = apiKey;
            Initialize(language);
        }
        private Airly(string apiKey, AirlyConfiguration configuration, AirlyLanguage? language)
        {
            ApiKey = apiKey;
            Configuration = configuration;
            Initialize(language);
        }

        public Airly(Airly airly) : this(airly.ApiKey, airly.Configuration, airly.Language) { }
        public Airly(Airly airly, AirlyConfiguration configuration) : this(airly.ApiKey, configuration, airly.Language) { }

        public Airly(string apiKey) : this(apiKey, language: null) { }
        public Airly(string apiKey, AirlyConfiguration configuration) : this(apiKey, configuration, null) { }

        private void Initialize(AirlyLanguage? language = null)
        {
            Rest = new(this);
            Installations = new(this);
            Meta = new(this);
            Measurements = new(this);

            if (language != null) Language = (AirlyLanguage) language;

            Utils.ValidateKey(ApiKey);
        }

        /// <summary>
        /// Destroying and Disposing the Airly API client. After this, the client can not be reused
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                Rest.Dispose();
                ApiKey = null;
                _isDisposed = true;
            }
        }

        public override string ToString() => ApiKey;
    }
}