using AviaExplorer.Models.Avia;
using AviaExplorer.ViewModels.Avia;
using Newtonsoft.Json;
using System;
using Xamarin.Forms;

namespace AviaExplorer.Views.Pages
{
    [QueryProperty(nameof(Direction), "data")]
    public partial class FlightDetailPage
    {
        private DirectionModel _direction;

        public string Direction
        {
            set => _direction = JsonConvert.DeserializeObject<DirectionModel>(Uri.UnescapeDataString(value));
        }

        public FlightDetailViewModel FlightDetailViewModel =>
            (FlightDetailViewModel)BindingContext;

        public FlightDetailPage() =>
            InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();

            FlightDetailViewModel?.SetDirectionCommand?.Execute(_direction);
            FlightDetailViewModel?.GetFlightsDataCommand?.Execute(null);
        }
    }
}