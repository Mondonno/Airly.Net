using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace AirlyAPI.handling
{
    // Simple wrapper for sempahores slim to handle threads in requests
    public class Waiter : SemaphoreSlim
    {
        public Waiter() : base(1){}
    }

    // The queuer make the same what do waiter but waiter is a core for queuer
    public class RequestQueuer
    {
        public List<RequestModule> queue { get; set; }
        public Waiter waiter { get; set; }
        public RESTManager manager { get; set; }

        public RequestQueuer(RESTManager manager)
        {
            this.manager = manager;
        }
        public async void push(RequestModule request)
        {
            await waiter.WaitAsync();

            try { this.make(request); }
            finally { waiter.Release(); }
        }

        private async void make(RequestModule request)
        {
            Handler handler = new Handler();
            AirlyResponse res = null;
            try
            {
                res = await request.MakeRequest();
            }
            catch (Exception ex)
            {

            }
        }
    }
}