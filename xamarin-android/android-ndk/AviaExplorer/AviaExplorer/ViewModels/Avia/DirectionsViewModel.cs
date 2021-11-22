using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Models.Utils;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace AviaExplorer.ViewModels.Avia
{
    public class DirectionsViewModel : BaseViewModel
    {
        #region Fields
        private readonly IAviaInfoService _aviaInfo;
        private readonly IAnalyticsService _analytics;
        private readonly ILanguageService _language;
        private readonly INavigationService _navigation;

        private IAsyncCommand _getSupportedDirectionsCommand;
        private IAsyncCommand<string> _navigateAirportCommand;
        private ICommand _clearSupportedDirectionsCommand;
        private ICommand _setOriginAirportCommand;

        private ObservableRangeCollection<DirectionModel> _directions =
            new ObservableRangeCollection<DirectionModel>();
        private List<DirectionModel> _pins = new List<DirectionModel>();
        private bool _directionsUpdating;
        #endregion

        #region Properties
        /// <summary>
        /// Directions available
        /// </summary>
        public ObservableRangeCollection<DirectionModel> Directions
        {
            get => _directions;
            set
            {
                _directions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Map pins
        /// </summary>
        public List<DirectionModel> Pins
        {
            get => _pins;
            set
            {
                _pins = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current chosen airport choice
        /// </summary>
        private AirportChoice OriginAirport { get; set; }

        /// <summary>
        /// Flag determines updating state
        /// </summary>
        public bool DirectionsUpdating
        {
            get => _directionsUpdating;
            set
            {
                _directionsUpdating = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Fetches supported directions
        /// </summary>
        public IAsyncCommand GetSupportedDirectionsCommand => _getSupportedDirectionsCommand
            ?? ( _getSupportedDirectionsCommand = new AsyncCommand(GetSupportedDirectionsAsync,
                _ => !DirectionsUpdating,
                e =>
                {
                    DirectionsUpdating = false;
                    _analytics.TrackError(e);
                }));

        /// <summary>
        /// Navigates to the next page
        /// </summary>
        public IAsyncCommand<string> NavigateAirportCommand => _navigateAirportCommand
            ?? (_navigateAirportCommand = new AsyncCommand<string>(NavigateAirportAsync));

        /// <summary>
        /// Clears supported directions for recycling event
        /// </summary>
        public ICommand ClearSupportedDirectionsCommand => _clearSupportedDirectionsCommand
            ?? (_clearSupportedDirectionsCommand = new Command(ClearSupportedDirections));

        /// <summary>
        /// Sets chosen airport choice
        /// </summary>
        public ICommand SetOriginAirportCommand => _setOriginAirportCommand
            ?? (_setOriginAirportCommand = new Command<AirportChoice>(iata => OriginAirport = iata));
        #endregion

        public DirectionsViewModel(IAviaInfoService aviaInfo,
                                   IAnalyticsService analytics,
                                   ILanguageService language,
                                   INavigationService navigation)
        {
            _aviaInfo = aviaInfo;
            _analytics = analytics;
            _language = language;
            _navigation = navigation;
        }

        #region Methods
        private async Task GetSupportedDirectionsAsync()
        {
            if (string.IsNullOrEmpty(OriginAirport.Name))
            {
                DirectionsUpdating = false;
                return;
            }

            Pins.Clear();
            Directions.Clear();

            var result = await _aviaInfo.GetSupportedDirectionsAsync(OriginAirport.Name, true, _language.Current)
                .ConfigureAwait(false);

            await Device.InvokeOnMainThreadAsync(() =>
            {
                Directions.AddRange(result.Directions
                    .Select(x => new DirectionModel
                    {
                        OriginIATA = result.Origin.IATA,
                        DestinationIATA = x.IATA,
                        OriginName = result.Origin.Name,
                        DestinationName = x.Name,
                        DestinationCountry = x.Country,
                        GeoPosition = new Position(
                            x.Coordinates.LastOrDefault(),
                            x.Coordinates.FirstOrDefault())
                    })
                    .Take(25));

                Pins = Directions.ToList();
            });

            DirectionsUpdating = false;
        }

        private Task NavigateAirportAsync(string name)
        {
            var destAirport = Directions.FirstOrDefault(x => x.DestinationName == name);
            var data = Uri.EscapeDataString(JsonConvert.SerializeObject(destAirport));
            return _navigation.NavigateToPageAsync($"flights?data={data}");
        }

        private void ClearSupportedDirections() =>
            Directions.Clear();
        #endregion
    }
}