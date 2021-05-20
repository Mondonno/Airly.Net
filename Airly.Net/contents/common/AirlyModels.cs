using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using AirlyNet.Utilities;

namespace AirlyNet.Models
{
    public class LocationArea
    {
        public LocationArea(Location sw, Location ne)
        {
            Sw = sw ?? throw new ArgumentNullException("sw");
            Ne = ne ?? throw new ArgumentNullException("ne");
        }

        public Location Sw { get; set; }
        public Location Ne { get; set; }

        public bool Contains(Location location) => GeoUtil.Contains(location.Lat, Sw.Lat, Ne.Lat) && GeoUtil.Contains(location.Lng, Sw.Lng, Ne.Lng);
        public Location GetBarycenter() => new Location(
            GeoUtil.GetMidpoint(Sw.Lat, Ne.Lat),
            GeoUtil.GetMidpoint(Sw.Lng, Ne.Lng)
        );

        public static bool operator ==(LocationArea one, LocationArea two) => one != null && two != null ? one.Ne == two.Ne && one.Sw == two.Sw : one == two;
        public static bool operator !=(LocationArea one, LocationArea two) => one != null && two != null ? one.Ne != two.Ne || one.Sw != two.Sw : one != two;

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => $"{Sw.Lat} {Sw.Lng}\n{Ne.Lat} {Ne.Lng}";
    }
    public class Location
    {
        public Location(double lat, double lng)
        {
            ParamsValidator.ThrowIfInfinity(lat, lng);

            Lat = lat;
            Lng = lng;
        }

        [JsonProperty("latitude")]
        public double Lat { get; set; }

        [JsonProperty("longitude")]
        public double Lng { get; set; }

        public static bool operator ==(Location one, Location two) => (!Equals(one, null) && !Equals(two, null)) ? one.Lng == two.Lng && one.Lat == two.Lat : Equals(one, two);
        public static bool operator !=(Location one, Location two) => (!Equals(one, null) && !Equals(two, null)) ? one.Lng != two.Lng || one.Lat != two.Lat : !Equals(one, two);

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => $"{Lat} {Lng}";
    }

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

    /* <<=============================>>
    *   Models for airly api responses
    *  <<=============================>>
    *
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

    public class Address
    {
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("displayAddress1")]
        public string DisplayAddressOne { get; set; }
        [JsonProperty("displayAddress2")]
        public string DisplayAddressTwo { get; set; }
    }

    public class Installation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        // can not check the locationId is always avaible because of this isn't documentated
        [JsonProperty("locationId")]
        public int? LocationId { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
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