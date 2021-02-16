using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirlyAPI.handling;

namespace AirlyAPI
{
    // //////////////////////////////////// TODO \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //  TODO: Zobaczenie na githubie projektu wrappera do discord API w C# (analiza)
    // \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\ TODO ////////////////////////////////////

    // Simple github.com/kyranet AsyncQueue wrapper in C#
    // Prevents lock-ups and infinity loops in threads
    public class Waiter
    {
        public List<Task> tasks { get; set; }

        public int remaining { get => tasks.Count; }

        public async Task wait()
        {
            Task next = this.remaining != 0 ? this.tasks[this.remaining] : new Task(() => { });
            
            this.tasks.Add((Task) next);

            next.Wait();
            next.Start();
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

        public RequestQueuer(RESTManager manager){
            this.manager = manager;
        }
        public async void push(RequestModule request)
        {
            await waiter.wait();

            try { this.make(request); }
            finally{ waiter.shift(); }
        }

        public void make(RequestModule request)
        {
            Handler handler = new Handler();

        }
    }

    public interface IBaseRouter { }
    
    // The routes is the wrapper for the Request Module with the prered useful methods
    // The reuqest manager is the manager of all API operations
    // :)
    public class RESTManager : BasicRoutes
    {
        private string apiKey { get; set; }
        public AirlyLanguage lang { get; set; }

        public RESTManager(AirlyProps airlyProperties, string key)
        {
            this.airlyProperties = airlyProperties;
            this.apiKey = key;
        }

        public RESTManager(Airly airly)
        {
            this.apiKey = airly.apiKey;
        }

        public string Endpoint {
            get { return this.Endpoint; }
            set { Endpoint = value; }
        }
        
        public AirlyProps airlyProperties
        {
            get { return this.airlyProperties; }
            set { airlyProperties = value; }
        }

        public string Cdn { get => $"cdn.{Utils.domain}"; }

        private object validateLang(object lang)
        {
            string air = nameof(AirlyLanguage);
            if (lang.GetType().ToString() == air) return lang;
            else return AirlyLanguage.en;
        }

        // Making the request to the API
        public async Task<AirlyResponse> request(string end, string method, object options = null)
        {
            var util = new Utils();

            // Simple wraps for the null options option
            string[] wrap = { };
            string[][] wrapped = { wrap };
            if (options == null) options = new RequestOptions(wrapped);

            var requestManager = new RequestModule(end, method, (RequestOptions) options, airlyProperties);
            requestManager.airlyProperties.API_KEY = airlyProperties.API_KEY;

            if (airlyProperties.API_KEY == null) this.airlyProperties.API_KEY = "";

            requestManager.setKey(apiKey);

            object lang = this.lang;
            requestManager.setLanguage((AirlyLanguage) validateLang(lang));

            var response = await requestManager.MakeRequest();

            string dateHeader = util.getHeader(response.headers, "Date");
            DateTime date = DateTime.Parse(dateHeader ?? DateTime.Now.ToString()); // If the date header is null setting the date for actual date

            return new AirlyResponse(response, date);
        }

        // Simple get wrapper
        public async Task<T> api<T>(string end, dynamic query) => Utils.ParseToClassJSON<T>((await request(end, "get", new RequestOptions(new Utils().ParseQuery(query)))).JSON);
    }

    public class BasicRoutes : IBaseRouter
    {
        public BasicRoutes(){}

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
