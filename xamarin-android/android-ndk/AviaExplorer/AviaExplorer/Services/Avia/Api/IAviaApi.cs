using Refit;
using System.Threading.Tasks;

namespace AviaExplorer.Services.Avia.Api
{
    public interface IAviaApi
    {
        /// <summary>
        /// Api method
        /// </summary>
        /// <param name="originIATA">Origin IATA code</param>
        /// <param name="oneWay">Is one way?</param>
        /// <param name="language">Current language</param>
        /// <returns></returns>
        [Get("/supported_directions.json")]
        Task<TResult> GetSupportedDirectionsAsync<TResult>([AliasAs("origin_iata")] string originIATA,
                                                     [AliasAs("one_way")] bool oneWay,
                                                     [AliasAs("locale")] string language);

        /// <summary>
        /// Api method
        /// </summary>
        /// <param name="originIATA">Origin IATA code</param>
        /// <param name="oneWay">Is one way?</param>
        /// <param name="language">Current language</param>
        /// <param name="periodDate">Period</param>
        /// <param name="direct">Has any transfers?</param>
        /// <param name="maxPrice">Maximum price</param>
        /// <param name="noVisa">Payment with visa?</param>
        /// <param name="schengen">Is under schengen zone?</param>
        /// <param name="needVisa">Needs visa?</param>
        /// <param name="minTripDurationInDays">Minimum days amount of trip</param>
        /// <param name="maxTripDurationInDays">Maximum days amount of trip</param>
        /// <returns></returns>
        [Get("/prices.json")]
        Task<TResult> GetFlightsDataAsync<TResult>([AliasAs("origin_iata")] string originIATA,
                                         [AliasAs("one_way")] bool oneWay,
                                         [AliasAs("locale")] string language,
                                         [AliasAs("period")] string periodDate,
                                         [AliasAs("direct")] bool direct,
                                         [AliasAs("price")] string maxPrice,
                                         [AliasAs("no_visa")] bool noVisa,
                                         [AliasAs("schengen")] bool schengen,
                                         [AliasAs("need_visa")] bool needVisa,
                                         [AliasAs("min_trip_duration_in_days")] string minTripDurationInDays,
                                         [AliasAs("max_trip_duration_in_days")] string maxTripDurationInDays);
    }
}