using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace AirlyAPI.handling
{
    // == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==
    //   TODO: Przekształcić zwracaną wartość z RequestModule na RawResponse i parsowanie zrobić z poziomu queuera (parse method)
    // == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==

    // Simple wrapper for semaphore slim to handle threads in requests
    public class Waiter : SemaphoreSlim
    {
        public Waiter() : base(0){} // Limiting to one action per push and one thread
    }

    // The queuer make the same what do waiter but waiter is a core for queuer
    public class RequestQueuer
    {
        public Waiter waiter { get; set; }
        public RESTManager manager { get; set; }

        public RequestQueuer(RESTManager manager)
        {
            this.manager = manager;
        }
        public async Task<string> Push(RequestModule request)
        {
            await waiter.WaitAsync();

            // Simple thread locks handlings
            try { return await this.Make(request); }
            finally { waiter.Release(); }
        }

        private async Task<string> Make(RequestModule request)
        {
            Handler handler = new Handler();
            AirlyResponse res;

            try
            {
                res = await request.MakeRequest();
            }
            catch (Exception ex)
            {
                throw new AirlyError(new HttpError(ex));
            }

            if (res == null || res.rawJSON == String.Empty) throw new HttpError("Can not resolve the Airly api response");

            ErrorModel convertedError = handler.getErrorFromJSON(res.JSON);

            int? succesor = convertedError.succesor;
            var convertCheck = !(succesor == 0 || succesor == null);
            //if (convertCheck) return succesor.ToString();

            //if(res.me)

            int statusCode = 111;
            handler.handleError(statusCode, res);

            return null;
        }
    }
}