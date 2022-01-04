using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using AirlyNet.Utilities;

namespace AirlyNet.Common
{
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
        /// EN Language (default)
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
}