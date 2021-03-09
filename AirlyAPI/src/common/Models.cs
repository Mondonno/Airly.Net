using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AirlyAPI.Utilities;

namespace AirlyAPI
{
    public enum AirlyNotFoundHandling
    {
        Error,
        Null
    }

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
        public string[][] Query { get; set; }
        public string Body { get; set; }
        public bool Auth { get; set; } = true;
        public bool Versioned { get; set; } = true; // True by deafult
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
            this.sw = sw ?? throw new ArgumentNullException("sw");
            this.ne = ne ?? throw new ArgumentNullException("ne");
        }

        public Location sw { get; set; }
        public Location ne { get; set; }

        public bool Contains(Location location) => GeoUtil.Contains(location.Lat, sw.Lat, ne.Lat) && GeoUtil.Contains(location.Lng, sw.Lng, ne.Lng);
        public Location GetBarycenter() => new Location(
            GeoUtil.GetMidpoint(sw.Lat, ne.Lat),
            GeoUtil.GetMidpoint(sw.Lng, ne.Lng)
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

    public class IndexLevel
    {
        [JsonProperty("maxValue")]
        public int? MaxValue { get; set; }
        [JsonProperty("minValue")]
        public int? MinValue { get; set; }

        [JsonProperty("values")]
        public string Values { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
    }

    public class Standard
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("pollutant")]
        public string Pollutant { get; set; }

        [JsonProperty("averaging")]
        public string Averaging { get; set; }
        [JsonProperty("limit")]
        public double? Limit { get; set; }

        [JsonProperty("percent")]
        public double? Percent { get; set; }
    }

    public class Index
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double? Value { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("advice")]
        public string Advice { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
    }

    public class IndexType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("levels")]
        public List<IndexLevel> Levels { get; set; }
    }

    public class UnitValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double? Value { get; set; }
    }

    public class Location
    {
        public Location(double lat, double lng)
        {
            this.Lat = lat;
            this.Lng = lng;
        }

        [JsonProperty("latitude")]
        public double Lat { get; set; }

        [JsonProperty("longitude")]
        public double Lng { get; set; }
    }

    public class Sponsor
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("logo")]
        public Uri Logo { get; set; }
        [JsonProperty("link")]
        public Uri Link { get; set; }
    }

    public class Adress
    {
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("number")]
        public int? Number { get; set; }

        [JsonProperty("displayAddress1")]
        public string DisplayAddress1 { get; set; }
        [JsonProperty("displayAddress2")]
        public string DisplayAddress2 { get; set; }
    }

    public class Installation
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("locationId")]
        public int? LocationId { get; set; }

        [JsonProperty("address")]
        public Adress Address { get; set; }
        [JsonProperty("location")]
        public Location Location { get; set; }
        [JsonProperty("sponsor")]
        public Sponsor Sponsor { get; set; }

        [JsonProperty("elevation")]
        public double Elevation { get; set; }
        [JsonProperty("airly")]
        public bool Airly { get; set; }
    }

    public class Measurement
    {
        [JsonProperty("current")]
        public SingleMeasurement Current { get; set; }

        [JsonProperty("history")]
        public List<SingleMeasurement> History { get; set; }
        [JsonProperty("forecast")]
        public List<SingleMeasurement> Forecast { get; set; }
    }

    public class MeasurementType
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    public class SingleMeasurement
    {
        [JsonProperty("fromDateTime")]
        public DateTime FromDateTime { get; set; }
        [JsonProperty("tillDateTime")]
        public DateTime TillDateTime { get; set; }

        [JsonProperty("values")]
        public List<UnitValue> Values { get; set; }
        [JsonProperty("indexes")]
        public List<Index> Indexes { get; set; }
        [JsonProperty("standards")]
        public List<Standard> Standards { get; set; }
    }
}