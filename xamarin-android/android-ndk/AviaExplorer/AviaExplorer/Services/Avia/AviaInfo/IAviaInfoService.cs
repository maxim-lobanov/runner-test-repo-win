using AviaExplorer.Models.Avia.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AviaExplorer.Services.Avia.AviaInfo
{
    public interface IAviaInfoService
    {
        Task<DirectionsResponseModel> GetSupportedDirectionsAsync(string originIATA,
                                                                  bool oneWay,
                                                                  string language);

        Task<List<FlightResponseModel>> GetFlightsDataAsync(string originIATA,
                                                       bool oneWay,
                                                       string language,
                                                       string periodDate,
                                                       bool direct,
                                                       string maxPrice,
                                                       bool noVisa,
                                                       bool schengen,
                                                       bool needVisa,
                                                       string minTripDurationInDays,
                                                       string maxTripDurationInDays);
    }
}