using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace AviaExplorer.Models.Avia.Api
{
    public class DirectionsResponseModel
    {
        [JsonProperty("origin")]
        public OriginDirection Origin { get; set; }

        [JsonProperty("directions")]
        public List<DesiredDirection> Directions { get; set; }

        #region Direction classes
        public abstract class Direction
        {
            [JsonProperty("iata")]
            public string IATA { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("coordinates")]
            public double[] Coordinates { get; set; }
        }

        public class OriginDirection : Direction { }

        public class DesiredDirection : Direction
        {
            [JsonProperty("direct")]
            public bool Direct { get; set; }
        }
        #endregion
    }

    public class FlightResponseModel
    {
        [JsonProperty("show_to_affiliates")]
        public bool ShowToAffiliates { get; set; }

        [JsonProperty("trip_class")]
        public byte TripClass { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("depart_date")]
        public string DepartDate { get; set; }

        [JsonProperty("return_date")]
        public string ReturnDate { get; set; }

        [JsonProperty("number_of_changes")]
        public uint NumberOfChanges { get; set; }

        [JsonProperty("value")]
        public ulong FlightPrice { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("ttl")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime TimeToLive { get; set; }

        [JsonProperty("distance")]
        public uint Distance { get; set; }

        [JsonProperty("actual")]
        public bool Actual { get; set; }
    }
}