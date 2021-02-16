using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlyAPI.handling
{
    // Simple github.com/kyranet AsyncQueue wrapper in C#
    // Prevents lock-ups and infinity loops in threads
    public class Waiter
    {
        public List<Task> tasks { get; set; }

        public int remaining { get => tasks.Count; }

        public async Task wait()
        {
            Task next = this.remaining != 0 ? this.tasks[this.remaining] : new Task(() => { });

            this.tasks.Add((Task)next);
            
            next.Start();
            next.Wait();
        }

        public void shift()
        {
            Task task = this.tasks[0];
            task = tasks.Find((e) => e == task);
            tasks.Remove(task);
            if (task != null) task.Dispose();
        }
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
            await waiter.wait();

            try { this.make(request); }
            finally { waiter.shift(); }
        }

        private void make(RequestModule request)
        {
            Handler handler = new Handler();
          
        }
    }
}