using System;
using System.Runtime.Serialization;

namespace AirlyNet.Handling.Errors
{
    [Serializable]
    public class BaseError : Exception
    {
        public BaseError() : base() { }
        public BaseError(string message) : base(message) { }
        public BaseError(string message, Exception innerException) : base(message, innerException) { }
        public BaseError(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class HttpError : BaseError
    {
        public HttpError(string content) : base(string.Format("New http error {0}", content)) { }
        public HttpError(Exception rawError) : base("New http error WITH the unknown stack trace", rawError) { }
    }

    [Serializable]
    public class AirlyError : BaseError
    {
        public AirlyError(string content) : base(string.Format("The airly get following error.\nMessage: {0}", content)) { }
        public AirlyError(HttpError error) : base(string.Format("Airly get the following http error {0}", error.ToString())) { }
        public AirlyError(string link, string content) : base(string.Format("The airly get following error.\nMessage: {0}", content)) => HelpLink = link;
    }

    [Serializable]
    public class InvalidApiKeyError : BaseError
    {
        public InvalidApiKeyError(string message) : base(string.Format("The provided Api Key is invalid\n{0}", message)) => Data.Add("Token", false);
        public InvalidApiKeyError() : this(string.Empty) { }
    }

    [Serializable]
    public class RateLimitError : BaseError
    {
        public RateLimitError(string message) : base(string.Format("Airly.Net get restricted by Airly API ratelimit\n{0}", message)) => Data.Add("RateLimited", true);
        public RateLimitError() : this(string.Empty) { }
    }

    [Serializable]
    public class ElementPermentlyReplacedException : BaseError
    {
        public ElementPermentlyReplacedException(string newLocation, string optionalMessage = "") : base($"Airly element got permently replaced {optionalMessage}")
        {
            Data.Add("location", newLocation ?? throw new ArgumentNullException("newLocation"));
        }
    }
}