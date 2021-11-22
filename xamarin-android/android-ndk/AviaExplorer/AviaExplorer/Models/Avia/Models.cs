using System;
using Xamarin.Forms.Maps;

namespace AviaExplorer.Models.Avia
{
    public class AirportChoice
    {
        public string Name { get; set; }

        public Position GeoPosition { get; set; }
    }

    public class DirectionModel
    {
        public string OriginName { get; set; }

        public string DestinationName { get; set; }

        public string CombinedName => $"{ OriginName } - { DestinationName }";

        public string DestinationCountry { get; set; }

        public string OriginIATA { get; set; }

        public string DestinationIATA { get; set; }

        public string CombinedIATA => $"{ OriginIATA } - { DestinationIATA }";

        public Position GeoPosition { get; set; }
    }

    public class FlightModel
    {
        public DateTime DepartureDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public ulong Price { get; set; }
    }
}