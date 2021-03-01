// It is not unit tests but this is my codes trash xD
// If you are looking for tests just look for the tests dir in the Project overlook
// And also you can not see this folder and file because the folder is in git ignore xD (so idk why I'm writing this lol)

using System;
using System.Collections.Generic;
using AirlyAPI;

// The namespace for AirlyAPI unit tests
namespace AirlyAPI.test
{
    public class unitTests
    {
        public void test()
        {
            //setLanguage(language == "en" ? AirlyLanguage.en : (language == "pl" ? AirlyLanguage.pl : AirlyLanguage.en));
            //string virtualHost = $"https://{API_DOMAIN}"; // Declaring the virtual host to get the query from URI
            //
            //Uri param = new Uri(virtualHost);
            //Uri constructedQuery = new Uri(string.Format("{0}{1}{2}", virtualHost, this.endPoint,
            //        (queryString != "" ? string.Format("?{0}",
            //        queryString.Replace("#", "%23").Replace("@", "%40")) // Replacing # and @ to the own URL code (Uri does not support # and @ in query)
            //        : ""))); // no need to parse the Query (eg. % === %25) (the Uri class do this for me)
        }
        //[TestMethod]
        public async void RunTests()
        {
            string apiKey = "121§324323c231xdsadI21OE";
            Airly airly = new Airly(apiKey);
            int id = 1;
            Installation results = null;

            // Recommended to use try catch when u want to use the redirect option
            // (Throwing error if the new Installation ID does not exists)
            try
            {
                 _ = results != null ? results = await airly.Installations.Info(id, true) : null;
            }
            catch (AirlyError ex)
            {
                // Console Line Writing the new succesor ID if the succesor is in the new of new Response
                // (To prevent infinity loops and the inifnity limit usage we throw error if the new succesor id does not exists)
                if(ex.Data["succesor"].ToString() != null)
                    Console.WriteLine($"The new succesor id: {ex.Data["succesor"]}");
            }
            if(results != null)
            {
                // Some actions with results
            }
        }
        public async void RunBasicAirlyTest()
        {
            string apiKey = "123213125sdfggsaete3123";
            Airly airly = new Airly(apiKey);

            double lat = 12.035;
            double lng = 13.01;
            double maxDistance = 12.334;

            var measurements = await airly.Measurments.Nearest(lat, lng, maxDistance, IndexQueryType.CAQI);
            List<DateTime> datetimes = new List<DateTime>();
            foreach (var item in measurements)
            {
                string rawdate = item.current.fromDateTime.ToString();
                DateTime date = DateTime.Parse(rawdate);
                datetimes.Add(date);
            }
        }
        public void RunInstallTests()
        {
            //string apiKey = "123213125sdfggsaete3123";
            //Airly airly = new Airly(apiKey);
            //double lat = 12.035;
            //double lng = 13.01;
            ////double maxDistance = 12.334;
            //var Measurements = await airly.Measurments.Point(lat, lng, IndexQueryType.AirlyCAQI);

        }

        // Running the request tests
        public static void RunRequest()
        {

            //string apiKey = "12233333";

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

//// Checking if the custom headers contains the api key and then setting them to the apiKey header
//            if (deafultHttpHeaders.Contains(API_KEY_HEADER_NAME)) {
//                string firstValue = moduleUtil.getHeader(deafultHttpHeaders, API_KEY_HEADER_NAME);

//_ = apiKey[0] == API_KEY_HEADER_NAME? apiKey[1] = firstValue : apiKey[1] = "";
//            }
//            else

//if (moduleUtil.Exists(customHeaders, API_KEY_HEADER_NAME) > -1)
//            {
//                string key = customHeaders[(moduleUtil.Exists(customHeaders, API_KEY_HEADER_NAME))][1];
//apiKey[1] = key;
//            }