using System.Threading.Tasks;
using XF.Material.Forms.UI.Dialogs;

namespace AviaExplorer.Services.Utils.Message
{
    public interface IMessageService
    {
        Task DisplayInfoAsync(string message);

        Task DisplayErrorDescOnlyAsync(string errorDesc);

        Task<bool> DisplayConfirmationAsync(string dialogName, string dialogDesc, string confirmLabel,
            string denyLabel);

        Task<string> DisplayInputAsync(string title = "", string message = "", string inputText = null,
            string inputPlaceholder = "");

        void DisplayToast(string info);

        Task DisplaySnackbarAsync(string info, int duration = 2750);

        Task<IMaterialModalPage> DisplayLoadingAsync(string message);
    }
}