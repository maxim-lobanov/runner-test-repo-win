using AviaExplorer.Services.Interfaces;
using Xamarin.Forms;

namespace AviaExplorer.iOS.Implementations
{
    public class Toast : IToast
    {
        public void ShowToast(string message) =>
            Device.InvokeOnMainThreadAsync(() =>
            {
            });
    }
}