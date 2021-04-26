using System;

using AirlyNet.Interactions;
using AirlyNet.Rest;
using AirlyNet.Utilities;

// todo zaimplementowanie CacheService do Airly.Net
// todo przetestowac nowy system obłsugi 301 (jak znajde replaced)

namespace AirlyNet
{
    public class Airly : IDisposable
    { 
        protected bool _isDisposed;

        public string ApiKey { get; private set; }

        private RESTManager Rest { get; set; }
        protected internal RestApiClient Client { get; private set; }

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
            ApiKey = apiKey; // key validation is on the Util.validateKey
            Initialize(language);
        }
        private Airly(string apiKey, AirlyConfiguration configuration, AirlyLanguage? language)
        {
            ApiKey = apiKey; // key validation is on the Util.validateKey
            Configuration = configuration ?? throw new ArgumentNullException("configuration");
            Initialize(language);
        }

        public Airly(Airly airly) : this(airly.ApiKey, airly.Configuration, airly.Language) { }
        public Airly(Airly airly, AirlyConfiguration configuration) : this(airly.ApiKey, configuration, airly.Language) { }

        public Airly(string apiKey) : this(apiKey, language: null) { }
        public Airly(string apiKey, AirlyConfiguration configuration) : this(apiKey, configuration, null) { }

        private void Initialize(AirlyLanguage? language = null)
        {
            RESTManager restManager = new(this);

            Client = new(restManager);
            Rest = restManager;

            Installations = new(this);
            Meta = new(this);
            Measurements = new(this);

            if (language != null) Language = (AirlyLanguage) language;

            Utils.ValidateKey(ApiKey);
        }

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