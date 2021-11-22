using AviaExplorer.ViewModels.Avia;
using Xamarin.Forms;

namespace AviaExplorer.Views.Pages
{
    public partial class OriginSelectionPage
    {
        private OriginSelectionViewModel OriginSelectionViewModel =>
            (OriginSelectionViewModel)BindingContext;

        public OriginSelectionPage() =>
            InitializeComponent();

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            OriginSelectionViewModel?.HideKeyboardCommand?.Execute(null);
        }

        private void MaterialTextField_Unfocused(object sender, FocusEventArgs e) =>
            OriginSelectionViewModel?.HideKeyboardCommand?.Execute(null);
    }
}