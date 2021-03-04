using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AirlyAPI.Utilities;

namespace AirlyAPI
{
    public enum AirlyLanguage
    {
        pl,
        en
    }

    public enum IndexQueryType
    {
        AirlyCAQI,
        CAQI,
        PJP
    }

    public class RequestOptions
    {
        public RequestOptions() { } // bypassing the required constructor
        public RequestOptions(string[][] query)
        {
            this.query = query;
        }

        public string[][] query { get; set; }
        public bool auth { get; set; } = true;
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

        public JToken JSON { get; } // Added JToken no raw json conversion
        public string rawJSON { get; }

        public DateTime timestamp { get; }
        public HttpResponseHeaders headers { get; }

        public HttpResponseMessage response { get; set; }
    }

    // Simple internal used RateLimitInformation object
    public class RateLimitInformation
    {
        public bool IsRateLimited { get; set; } 

        public int? RateLimitDiffrent { get; set; }
        public int PerDays { get; set; }
        public int PerGlobal { get; set; }
    }

    // Handler raw response (with converted JSON)
    public class RawHandlerResponse
    {
        public JToken convertedJSON { get; set; }
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

    // Area model class
    public class LocationArea
    {
        public LocationArea(Location sw, Location ne)
        {
            this.sw = sw;
            this.ne = ne;
        }

        public Location sw { get; set; }
        public Location ne { get; set; }

        public bool Contains(Location location) => GeoUtil.Contains(location.lat, sw.lat, ne.lat) && GeoUtil.Contains(location.lng, sw.lng, ne.lng);
        public Location GetBarycenter() => new Location(
            GeoUtil.GetMidpoint(sw.lat, ne.lat),
            GeoUtil.GetMidpoint(sw.lng, ne.lng)
        );
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
        public int? maxValue { get; set; }
        public int? minValue { get; set; }

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
        public Location(double lat, double lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        [JsonProperty("latitude")]
        public double lat { get; set; }

        [JsonProperty("longitude")]
        public double lng { get; set; }
    }

    public class Sponsor
    {
        public int id { get; set; }

        public string name { get; set; }
        public string displayName { get; set; }

        public string description { get; set; }

        public Uri logo { get; set; }
        public Uri link { get; set; }
    }

    public class Adress
    {
        public string country { get; set; } 
        public string city { get; set; }
        public string street { get; set; }
        public int? number { get; set; }

        public string displayAddress1 { get; set; }
        public string displayAddress2 { get; set; }
    }

    public class Installation
    {
        public int id { get; set; }
        public int locationId { get; set; }

        public Adress address { get; set; }
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
        public string label { get; set; }
        public string unit { get; set; }
    }

    public class SingleMeasurement
    {
        public DateTime fromDateTime { get; set; }
        public DateTime tillDateTime { get; set; }

        public List<Value> values { get; set; }
        public List<Index> indexes { get; set; }
        public List<Standard> standards { get; set; }
    }
}
