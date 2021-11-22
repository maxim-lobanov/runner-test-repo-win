using AviaExplorer.Models.Avia;
using AviaExplorer.ViewModels.Avia;
using Newtonsoft.Json;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace AviaExplorer.Views.Pages
{
    [QueryProperty(nameof(Airport), "data")]
    public partial class DirectionsPage
    {
        private AirportChoice _airport;

        public string Airport
        {
            set => _airport = JsonConvert.DeserializeObject<AirportChoice>(Uri.UnescapeDataString(value));
        }

        private DirectionsViewModel FlightsViewModel =>
            (DirectionsViewModel)BindingContext;

        public DirectionsPage() =>
            InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();

            FlightsViewModel?.SetOriginAirportCommand?.Execute(_airport);
            FlightsViewModel?.GetSupportedDirectionsCommand?.Execute(null);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(_airport.GeoPosition, new Distance(2400)));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            FlightsViewModel?.ClearSupportedDirectionsCommand?.Execute(null);
        }

        private void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            var pin = (Pin)sender;
            FlightsViewModel?.NavigateAirportCommand?.Execute(pin.Label);
        }
    }
}