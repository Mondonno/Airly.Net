using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AirlyAPI
{
    // Types for Airly API (eg. enums of pollusions)
    public static class Types
    {
        public enum PollusionsUnits
        {
            Celcjusza,
            Precent,
            HpA,
            µgm,
            kilometerForHour, 
            Stopien,
        }

        public enum RegistredPollusions
        {
            PM10
        }

        public enum RequestMethod
        {
            GET,
            POST,
            DELETE,
        }
    }

    public class AirlyProps
    {
        public string API_KEY { get; set; }
        public string LANG { get; set; }
        public string RateLimit { get; set; }
        public string RateLimitMax { get; set; }
        //public Types airlyTypes { get; set; }
        //public AirlyProps(Types atypes) { this.airlyTypes = atypes; }
    }

    public class RequestOptions
    {
        public RequestOptions(string[][] query)
        {
            this.query = query;
        }

        public string[][] query { get; set; }
    }

    public class AirlyResponse
    {
        public AirlyResponse(dynamic JSON, HttpResponseHeaders headers, string rawJSON, DateTime timestamp) {
            this.JSON = JSON;
            this.rawJSON = rawJSON;
            this.headers = headers;
            this.timestamp = timestamp;
        }

        public AirlyResponse(AirlyResponse response, DateTime timestamp)
        {
            this.JSON = response.JSON;
            this.rawJSON = response.rawJSON;
            this.headers = response.headers;
            this.timestamp = timestamp;
        }

        public dynamic JSON { get; }
        public string rawJSON { get; }

        public DateTime timestamp { get; }
        public HttpResponseHeaders headers { get; }

        public HttpResponseMessage response { get; }
    }

    // Handler raw response (with converted JSON)
    public class RawHandlerResponse
    {
        public dynamic convertedJSON { get; set; }
        public HttpResponseMessage response { get; set; }
    }

    // Raw Response (for RequestModule.cs) with raw (in string) JSON
    public class RawResponse
    {
        public RawResponse(HttpResponseMessage response, string rawJSON)
        {
            this.response = response;
            this.rawJSON = rawJSON;
        }

        public HttpResponseMessage response { get; set; }
        public string rawJSON { get; set; }
    }

    public enum AirlyLanguage
    {
        pl = 0,
        en = 1
    }

    public enum IndexQueryType
    {
        AirlyCAQI,
        CAQI,
        PJP
    }

    /* <<=============================>>
    *   Models for airly api responses
    *  <<=============================>>
    *    In use:
    * * Index Type
    * * Measurment Type
    * * Measurment
    * * Single Measurment
    * * Installation
    */
    public class Level
    {
        public string values { get; set; }
        public string level { get; set; }
        public string description { get; set; }
        public string color { get; set; }
    }

    public class Standard
    {
        public string name { get; set; }
        public string pollutant { get; set; }

        public double limit { get; set; }
        public double percent { get; set; }
    }

    public class Index
    {
        public string name { get; set; }
        public double value { get; set; }

        public string level { get; set; }

        public string description { get; set; }
        public string advice { get; set; }

        public string color { get; set; }
    }

    public class IndexType
    {
        public string name { get; set; }
        public List<Level> levels { get; set; }
    }

    public class Value
    {
        public string name { get; set; }
        public double value { get; set; }
    }

    public class Location
    {
        public int lat { get; set; }
        public int lng { get; set; }
    }

    public class LocationArea
    {
        public Location sw { get; set; }
        public Location ne { get; set; }
    }

    public class Sponsor
    {
        public string name { get; set; }
        public string description { get; set; }
        public string logo { get; set; }
        public string link { get; set; }
    }

    public class Adress
    {
        public string country { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public int number { get; set; }

        public string displayAddress1 { get; set; }
        public string displayAddress2 { get; set; }
    }

    public class Installation
    {
        public int id { get; set; }
        public int locationId { get; set; }

        public Adress adress { get; set; }
        public Location location { get; set; }
        public Sponsor sponsor { get; set; }

        public double elevation { get; set; }
        public bool airly { get; set; }
    }

    public class Measurement
    {
        public SingleMeasurement current { get; set; }

        public List<SingleMeasurement> history { get; set; }
        public List<SingleMeasurement> forecast { get; set; }
    }

    public class MeasurementType
    {
        public string name { get; set; }
        public string value { get; set; }
        public string unit { get; set; }
    }

    public class SingleMeasurement
    {
        public string fromDateTime { get; set; }
        public string tillDateTime { get; set; }

        public List<Value> values { get; set; }
        public List<Index> indexes { get; set; }
        public List<Standard> standards { get; set; }
    }

}
