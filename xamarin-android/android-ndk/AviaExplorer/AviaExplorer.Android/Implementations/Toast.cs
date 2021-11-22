using Android.Widget;
using AviaExplorer.Services.Interfaces;
using Xamarin.Forms;
using Application = Android.App.Application;
using AToast = Android.Widget.Toast;

namespace AviaExplorer.Droid.Implementations
{
    public class Toast : IToast
    {
        public void ShowToast(string message) =>
            Device.InvokeOnMainThreadAsync(() =>
                AToast.MakeText(Application.Context, message, ToastLength.Short).Show());
    }
}