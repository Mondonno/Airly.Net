using System;

using AirlyNet.Rest;
using AirlyNet.Utilities;
using AirlyNet.Interactions.Structures;
using AirlyNet.Common;

namespace AirlyNet
{
    /// <summary>
    /// Use this class to interact with the Airly API, main entry of Airly.Net
    /// </summary>
    public class Airly : IDisposable
    {
        protected bool _isDisposed;

        /// <summary>
        /// Api key used to authenticate when creating request
        /// </summary>
        public string ApiKey { get; private set; }

        private RESTManager Rest { get; set; }
        protected internal RestApiClient Client { get; private set; }

        /// <summary>
        /// Manager on the installations route. Use it when getting informations about Airly Installations
        /// </summary>
        public Installations Installations { get; private set; }

        /// <summary>
        /// Manager on measurements route. You can use it when you need to download installation measurment.
        /// </summary>
        public Measurements Measurements { get; private set; }

        /// <summary>
        /// Manager on Meta route. Methods here returning mainly some Airly API Constants
        /// </summary>
        public Meta Meta { get; private set; }

        /// <summary>
        /// Custom airly configuration.
        /// </summary>
        public AirlyConfiguration Configuration { get; set; } = new AirlyConfiguration();

        /// <summary>
        /// Used when sending request to API. Specfies in what language the response will return.
        /// </summary>
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

        /// <summary>
        /// Disposing all resources and unmanaged memory from Airly Client. After doing that you can not use this instance of Airly again.
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

        /// <summary>
        /// When used, returns the Airly api key
        /// </summary>
        /// <returns>ApiKey</returns>
        public override string ToString() => ApiKey;
    }
}