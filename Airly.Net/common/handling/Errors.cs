using System;
using System.Runtime.Serialization;

namespace AirlyNet.Handling.Exceptions
{
    [Serializable]
    public class BaseException : Exception
    {
        public BaseException() : base() { }
        public BaseException(string message) : base(message) { }
        public BaseException(string message, Exception innerException) : base(message, innerException) { }
        public BaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class HttpException : BaseException
    {
        public HttpException(string content) : base(string.Format("New http Exception {0}", content)) { }
        public HttpException(Exception rawException) : base("New http Exception WITH the unknown stack trace", rawException) { }
    }

    [Serializable]
    public class AirlyException : BaseException
    {
        public AirlyException(string content) : base(string.Format("The airly get following Exception.\nMessage: {0}", content)) { }
        public AirlyException(HttpException Exception) : base(string.Format("Airly get the following http Exception {0}", Exception.ToString())) { }
        public AirlyException(string link, string content) : base(string.Format("The airly get following Exception.\nMessage: {0}", content)) => HelpLink = link;
    }

    [Serializable]
    public class InvalidApiKeyException : BaseException
    {
        public InvalidApiKeyException(string message) : base(string.Format("The provided Api Key is invalid\n{0}", message)) => Data.Add("Token", false);
        public InvalidApiKeyException() : this(string.Empty) { }
    }

    [Serializable]
    public class RateLimitException : BaseException
    {
        public RateLimitException(string message) : base(string.Format("Airly.Net get restricted by Airly API ratelimit\n{0}", message)) => Data.Add("RateLimited", true);
        public RateLimitException() : this(string.Empty) { }
    }

    [Serializable]
    public class ElementPermentlyReplacedException : BaseException
    {
        public ElementPermentlyReplacedException(string newLocation, string optionalMessage = "") : base($"Airly element got permently replaced {optionalMessage}")
        {
            Data.Add("location", newLocation ?? throw new ArgumentNullException("newLocation"));
        }
    }
}