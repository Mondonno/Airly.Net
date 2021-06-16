using System;
using System.Threading.Tasks;
using AirlyNet.Rest;
using AirlyNet.Rest.Typings;

namespace AirlyNet.Handling
{
    public class RequestQueueHandler : IDisposable
    {
        private readonly RequestQueueManager QueueManager = new RequestQueueManager();
        private RESTManager RestManager { get; set; }

        public bool Inactive() => QueueManager.Inactive();
        public bool RateLimited() => QueueManager.RateLimited();
        public int Queuers() => QueueManager.Handlers();

        public RequestQueueHandler(RESTManager rest) => RestManager = rest;
        public Task<RestResponse> Queue(string route, RestRequest request)
        {
            RequestQueuer handler = QueueManager.Get(route);

            if (handler == null)
            {
                handler = new RequestQueuer(RestManager);
                QueueManager.Set(route, handler);
            }

            return handler.Push(request);
        }
        public void UnQueue(string route)
        {
            RequestQueuer queuer = QueueManager.Get(route);
            if (queuer == null) return;

            queuer.Dispose();
            QueueManager.Remove(route);
        }

        public void Dispose() => QueueManager.Dispose();
    }
}