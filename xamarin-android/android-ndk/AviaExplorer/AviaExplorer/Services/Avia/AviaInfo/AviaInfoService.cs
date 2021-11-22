using AviaExplorer.Models.Avia.Api;
using AviaExplorer.Services.Avia.Api;
using AviaExplorer.Services.Avia.RestClient;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AviaExplorer.Services.Avia.AviaInfo
{
    public class AviaInfoService : IAviaInfoService
    {
        private readonly IAviaApi _avia;

        public AviaInfoService(IRestClientProvider client)
        {
            _avia = RestService.For<IAviaApi>(client.BetrouteClient);
        }

        public Task<List<FlightResponseModel>> GetFlightsDataAsync(string originIATA,
            bool oneWay, string language, string periodDate, bool direct,
            string maxPrice, bool noVisa, bool schengen, bool needVisa,
            string minTripDurationInDays, string maxTripDurationInDays) =>
            _avia.GetFlightsDataAsync<List<FlightResponseModel>>(originIATA, 
                oneWay, language, periodDate, direct,
                maxPrice, noVisa, schengen, needVisa,
                minTripDurationInDays, maxTripDurationInDays);

        public Task<DirectionsResponseModel> GetSupportedDirectionsAsync(
            string originIATA, bool oneWay, string language) =>
            _avia.GetSupportedDirectionsAsync<DirectionsResponseModel>(originIATA, oneWay, language);
    }
}