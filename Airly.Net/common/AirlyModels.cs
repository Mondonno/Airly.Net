using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using AirlyNet.Utilities;

namespace AirlyNet.Models
{
    /// <summary>
    /// Location area, created from to Location objects.
    /// </summary>
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

    /// <summary>
    /// Main Location object used when interacting with Airly.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Create new location instance by passing Latidude nad Longatidude
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
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

    /// <summary>
    /// Represents the resource not found Error handling
    /// </summary>
    public enum AirlyNotFoundHandling
    {
        /// <summary>
        /// Throwing error when 404 throwed
        /// </summary>
        Error,
        /// <summary>
        /// Returning null when 404 throwed (deafult)
        /// </summary>
        Null
    }

    /// <summary>
    /// Represents the Language used when requesting to API
    /// </summary>
    public enum AirlyLanguage
    {
        /// <summary>
        /// PL Language
        /// </summary>
        PL,
        /// <summary>
        /// EN Language (deafult)
        /// </summary>
        EN
    }

    /// <summary>
    /// Index Query Type that specifies the returing measurment Index 
    /// </summary>
    public enum AirlyIndexQueryType
    {
        /// <summary>
        /// Airly CAQI Index (deafult)
        /// </summary>
        AirlyCAQI,
        /// <summary>
        /// CAQI used in Europe, recommended by WHO
        /// </summary>
        CAQI,
        /// <summary>
        /// Polish measurments index
        /// </summary>
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

    public class AirIndexLevel
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

    public class AirStandard
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

    public class AirIndex
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

    public class AirIndexType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("levels")]
        public List<AirIndexLevel> Levels { get; set; }
    }

    public class AirUnitValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double? Value { get; set; }
    }

    public class AirSponsor
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

    public class AirAddress
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

    public class AirInstallation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        // can not check the locationId is always avaible because of this isn't documentated
        [JsonProperty("locationId")]
        public int? LocationId { get; set; }

        [JsonProperty("address")]
        public AirAddress Address { get; set; }
        [JsonProperty("location")]
        public Location Location { get; set; }
        [JsonProperty("sponsor")]
        public AirSponsor Sponsor { get; set; }

        [JsonProperty("elevation")]
        public double Elevation { get; set; }
        [JsonProperty("airly")]
        public bool Airly { get; set; }
    }

    public class AirMeasurement
    {
        [JsonProperty("current")]
        public AirSingleMeasurement Current { get; set; }

        [JsonProperty("history")]
        public List<AirSingleMeasurement> History { get; set; }
        [JsonProperty("forecast")]
        public List<AirSingleMeasurement> Forecast { get; set; }
    }

    public class AirMeasurementType
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    public class AirSingleMeasurement
    {
        [JsonProperty("fromDateTime")]
        public DateTime FromDateTime { get; set; }
        [JsonProperty("tillDateTime")]
        public DateTime TillDateTime { get; set; }

        [JsonProperty("values")]
        public List<AirUnitValue> Values { get; set; }
        [JsonProperty("indexes")]
        public List<AirIndex> Indexes { get; set; }
        [JsonProperty("standards")]
        public List<AirStandard> Standards { get; set; }
    }
}