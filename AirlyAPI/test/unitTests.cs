using System;
using System.Collections.Generic;
using AirlyAPI;

// The namespace for AirlyAPI unit tests
namespace AirlyAPI.test
{
    public class unitTests
    {
        //[TestMethod]
        public void RunTests()
        {
            
        }
        public async void RunBasicAirlyTest()
        {
            string apiKey = "123213125sdfggsaete3123";
            Airly airly = new Airly(apiKey);

            double lat = 12.035;
            double lng = 13.01;
            double maxDistance = 12.334;

            var measurements = await airly.measurments.Nearest(lat, lng, maxDistance, Interactions.Measurments.IndexType.CAQI);
            List<DateTime> datetimes = new List<DateTime>();
            foreach (var item in measurements)
            {
                string rawdate = item.current.fromDateTime;
                DateTime date = DateTime.Parse(rawdate);
                datetimes.Add(date);
            }
        }
        public async void RunInstallTests()
        {
            string apiKey = "123213125sdfggsaete3123";
            Airly airly = new Airly(apiKey);
            double lat = 12.035;
            double lng = 13.01;
            double maxDistance = 12.334;
            var Measurements = await airly.measurments.Point(lat, lng, Interactions.Measurments.IndexType.AirlyCAQI);

        }

        // Running the request tests
        public static void RunRequest()
        {

            string apiKey = "o0utpKmGeTbxbwnVcER8KFpJi9EBilvu";

            //var properties = new AirlyProps(new Types());
            //properties.API_KEY = string.Format("{0}", apiKey);

            //var requestResponse = new RequestModule("meta", "", null, properties);

            //Console.WriteLine(requestResponse);
        }
        public static void Run()
        {
            RunRequest(); 
        }
        public static void Main(string[] args)
        { 
            Console.WriteLine("Started Unit Tests");
            Run();
        }
    }
}
