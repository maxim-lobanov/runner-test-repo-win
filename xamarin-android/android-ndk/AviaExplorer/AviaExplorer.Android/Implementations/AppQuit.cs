using Android.App;
using AviaExplorer.Services.Interfaces;

namespace AviaExplorer.Droid.Implementations
{
    public class AppQuit : IAppQuit
    {
        public void Quit()
        {
            ((Activity)Xamarin.Forms.Forms.Context).FinishAffinity();
        }
    }
}