using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Models.Utils;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Interfaces;
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

namespace AviaExplorer.ViewModels.Avia
{
    /// <summary>
    /// Handles origin IATA selection
    /// </summary>
    public class OriginSelectionViewModel : BaseViewModel
    {
        #region Fields
        private readonly IAviaInfoService _aviaInfo;
        private readonly IAnalyticsService _analytics;
        private readonly ILanguageService _language;
        private readonly IKeyboard _keyboard;
        private readonly INavigationService _navigation;

        private IAsyncCommand _getChoicesCommand;
        private IAsyncCommand<AirportChoice> _navigateToFlightsCommand;
        private ICommand _findAndNavigateCommand;
        private ICommand _clearChoicesCommand;
        private ICommand _filterOriginCommand;
        private ICommand _hideKeyboardCommand;

        private string _originIATA = "TOF";
        private List<AirportChoice> _choices = new List<AirportChoice>();
        private ObservableRangeCollection<AirportChoice> _availableChoices = 
            new ObservableRangeCollection<AirportChoice>();
        private bool _choicesUpdating;
        #endregion

        #region Properties
        /// <summary>
        /// Entered origin IATA code
        /// </summary>
        public string OriginIATA
        {
            get => _originIATA;
            set
            {
                _originIATA = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Back property: used for storing all choices
        /// </summary>
        public List<AirportChoice> Choices
        {
            get => _choices;
            set
            {
                _choices = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Choices which are displayed to the user
        /// </summary>
        public ObservableRangeCollection<AirportChoice> AvailableChoices
        {
            get => _availableChoices;
            set
            {
                _availableChoices = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Flag determines updating state
        /// </summary>
        public bool ChoicesUpdating
        {
            get => _choicesUpdating;
            set
            {
                _choicesUpdating = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Fetches choices
        /// </summary>
        public IAsyncCommand GetChoicesCommand => _getChoicesCommand ?? (_getChoicesCommand =
            new AsyncCommand(GetChoicesAsync,
                _ => !ChoicesUpdating,
                e =>
                {
                    ChoicesUpdating = false;
                    _analytics.TrackError(e);
                }));

        /// <summary>
        /// Navigates to the next page
        /// </summary>
        public IAsyncCommand<AirportChoice> NavigateToFlightsCommand => _navigateToFlightsCommand 
            ?? (_navigateToFlightsCommand = new AsyncCommand<AirportChoice>(NavigateToFlightsAsync));

        /// <summary>
        /// Finds the first choice and navigates to the next page
        /// </summary>
        public ICommand FindAndNavigateCommand => _findAndNavigateCommand ?? (_findAndNavigateCommand =
            new Command(FindAndNavigate));

        /// <summary>
        /// Clears choices for recycling event
        /// </summary>
        public ICommand ClearChoicesCommand => _clearChoicesCommand ?? (_clearChoicesCommand =
            new Command(ClearChoices));

        /// <summary>
        /// Filters choices as the user continues to input
        /// </summary>
        public ICommand FilterOriginCommand => _filterOriginCommand ?? (_filterOriginCommand =
            new Command<string>(FilterOrigin));

        /// <summary>
        /// Hides keyboard when navigated
        /// </summary>
        public ICommand HideKeyboardCommand => _hideKeyboardCommand ?? (_hideKeyboardCommand =
            new Command(() => _keyboard.HideKeyboard()));
        #endregion

        public OriginSelectionViewModel(IAviaInfoService aviaInfo,
                                        IKeyboard keyboard,
                                        IAnalyticsService analytics,
                                        ILanguageService language,
                                        INavigationService navigation)
        {
            _aviaInfo = aviaInfo;
            _analytics = analytics;
            _language = language;
            _keyboard = keyboard;
            _navigation = navigation;

            GetChoicesCommand?.Execute(null);
        }

        #region Methods
        private void FilterOrigin(string text)
        {
            var filterData = text;
            AvailableChoices.Clear();
            FilterOriginCommand?.CanExecute(false);
            if (string.IsNullOrEmpty(filterData))
            {
                AvailableChoices.AddRange(Choices);
                FilterOriginCommand?.CanExecute(true);
                return;
            }
            AvailableChoices.AddRange(Choices.Where(x => x.Name.StartsWith(text)));
            FilterOriginCommand?.CanExecute(true);
        }

        private void FindAndNavigate()
        {
            if (string.IsNullOrEmpty(OriginIATA)) return;
            var item = Choices.FirstOrDefault(x => x.Name.StartsWith(OriginIATA));
            NavigateToFlightsCommand?.Execute(item);
        }

        private async Task GetChoicesAsync()
        {
            if (string.IsNullOrEmpty(OriginIATA))
            {
                ChoicesUpdating = false;
                return;
            }

            AvailableChoices.Clear();
            Choices.Clear();

            var result = await _aviaInfo.GetSupportedDirectionsAsync(OriginIATA, true, _language.Current)
                .ConfigureAwait(false);

            await Device.InvokeOnMainThreadAsync(() =>
            {
                Choices = result.Directions
                    .Select(x => new AirportChoice
                    {
                        Name = x.IATA,
                        GeoPosition = new Xamarin.Forms.Maps.Position(
                            x.Coordinates.LastOrDefault(),
                            x.Coordinates.FirstOrDefault())
                    })
                    .Take(25)
                    .Distinct()
                    .OrderBy(x => x.Name)
                    .ToList();

                AvailableChoices.AddRange(Choices);
            });

            ChoicesUpdating = false;
        }

        private void ClearChoices() =>
            AvailableChoices.Clear();

        private Task NavigateToFlightsAsync(AirportChoice origin)
        {
            if (origin is null) return Task.CompletedTask;
            OriginIATA = origin.Name;
            var data = Uri.EscapeDataString(JsonConvert.SerializeObject(origin));
            return _navigation.NavigateToPageAsync($"directions?data={data}", false);
        }
        #endregion
    }
}