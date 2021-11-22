using AviaExplorer.Services.Utils.Settings;
using System;
using System.Net.Http;

namespace AviaExplorer.Services.Avia.RestClient
{
    public class RestClientProvider : IRestClientProvider
    {
        private readonly HttpClientHandler _handler;

        public HttpClient BetrouteClient { get; private set; }

        public RestClientProvider(ISettingsService settings)
        {
            _handler = new HttpClientHandler
            {
                UseCookies = true,
                UseProxy = false
            };

            BetrouteClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri(settings.DefaultApiUrl)
            };
        }
    }
}