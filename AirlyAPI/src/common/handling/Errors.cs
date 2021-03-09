using System;
using System.Runtime.Serialization;

namespace AirlyAPI.Handling
{
    [Serializable]
    public class BaseError : Exception
    {
        public BaseError() : base() { }
        public BaseError(string message) : base(message) { }
        public BaseError(string message, Exception innerException) : base(message, innerException) { }
        public BaseError(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class HttpError : BaseError
    {
        public HttpError(string content) : base(string.Format("[HTTP_ERROR] New http (web) error {0}", content)) { }
        public HttpError(Exception rawError) : base("[HTTP_ERROR] New http(web) error WITH the unknown stack trace", rawError) { }
    }

    public class AirlyError : BaseError
    {
        public AirlyError(string content) : base(string.Format("The airly get following error.\nMessage: {0}", content)) { }
        public AirlyError(HttpError error) : base(string.Format("Airly get the following http error {0}", error.ToString())) { }
        public AirlyError(string link, string content) : base(string.Format("The airly get following error.\nMessage: {0}", content))
        {
            this.HelpLink = link;
        }
    }
}