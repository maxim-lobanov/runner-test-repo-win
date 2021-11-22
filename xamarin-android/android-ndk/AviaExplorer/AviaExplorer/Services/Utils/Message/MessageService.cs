using AviaExplorer.Resources;
using AviaExplorer.Services.Interfaces;
using AviaExplorer.Services.Utils.Settings;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace AviaExplorer.Services.Utils.Message
{
    public class MessageService : IMessageService
    {
        #region Fields
        private readonly IToast _toast;
        private readonly ISettingsService _settings;
        #endregion

        #region Properties
        public static MaterialLoadingDialogConfiguration MaterialLoadingConfiguration =>
            new MaterialLoadingDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#20BC9B"),
                MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#2c3337").MultiplyAlpha(0.32)
            };

        public static MaterialLoadingDialogConfiguration BlackMaterialLoadingConfiguration =>
            new MaterialLoadingDialogConfiguration
            {
                BackgroundColor = Color.Black,
                MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32)
            };

        public static MaterialAlertDialogConfiguration MaterialAlertConfiguration =>
            new MaterialAlertDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#20BC9B"),
                MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32)
            };

        public static MaterialSimpleDialogConfiguration MaterialSimpleDialogConfiguration =>
            new MaterialSimpleDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#20BC9B"),
                TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                TextColor = Color.White
            };

        public static MaterialInputDialogConfiguration MaterialInputConfiguration =>
            new MaterialInputDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#20BC9B"),
                MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                InputTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                InputPlaceholderColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8)
            };

        public static MaterialInputDialogConfiguration MaterialPasswordInputConfiguration =>
            new MaterialInputDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#20BC9B"),
                MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                CornerRadius = 8,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                InputTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                InputPlaceholderColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                InputType = XF.Material.Forms.UI.MaterialTextFieldInputType.Password
            };

        public static MaterialSnackbarConfiguration MaterialSnackBarConfig =>
            new MaterialSnackbarConfiguration
            {
                BackgroundColor = (Color)Application.Current.Resources["accentColor"],
                MessageTextColor = Color.White
            };

        public static MaterialSnackbarConfiguration BlackMaterialSnackBarConfig =>
            new MaterialSnackbarConfiguration
            {
                BackgroundColor = Color.Black,
                MessageTextColor = Color.White
            };
        #endregion

        #region Constructor
        public MessageService(IToast toast,
                              ISettingsService settings)
        {
            _toast = toast;
            _settings = settings;
        }
        #endregion

        #region Methods
        public Task<bool> DisplayConfirmationAsync(string dialogName, string dialogDesc, string confirmLabel, string denyLabel)
        {
            var tcs = new TaskCompletionSource<bool>();
            Device.InvokeOnMainThreadAsync(async () =>
            {
                var result = await MaterialDialog.Instance.ConfirmAsync(
                    dialogDesc,
                    dialogName,
                    "OK",
                    AppResources.cancel,
                    MaterialAlertConfiguration);
                tcs.SetResult(result.GetValueOrDefault());
            });
            return tcs.Task;
        }

        public Task<string> DisplayInputAsync(string title = "", string message = "", string inputText = null, string inputPlaceholder = "") =>
            MaterialDialog.Instance.InputAsync(
                title,
                message,
                inputText,
                inputPlaceholder,
                "OK",
                AppResources.cancel,
                MaterialInputConfiguration);

        public Task DisplayErrorDescOnlyAsync(string errorDesc) =>
            MaterialDialog.Instance.AlertAsync(errorDesc, MaterialAlertConfiguration);

        public Task DisplayInfoAsync(string message) =>
            MaterialDialog.Instance.AlertAsync(message, MaterialAlertConfiguration);

        public Task DisplaySnackbarAsync(string info, int duration = 1500) =>
            MaterialDialog.Instance.SnackbarAsync(info, duration);

        public void DisplayToast(string info) =>
            _toast.ShowToast(info);

        public Task<IMaterialModalPage> DisplayLoadingAsync(string message) =>
            MaterialDialog.Instance.LoadingDialogAsync(message, MaterialLoadingConfiguration);
        #endregion
    }
}