using System.Net.Http;

namespace AviaExplorer.Services.Avia.RestClient
{
    public interface IRestClientProvider
    {
        HttpClient BetrouteClient { get; }
    }
}