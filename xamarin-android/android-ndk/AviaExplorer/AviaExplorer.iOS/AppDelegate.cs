using AviaExplorer.iOS.Implementations;
using AviaExplorer.Services.Interfaces;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using UIKit;

namespace AviaExplorer.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            FFImageLoading.FormsHandler.Init();
            XF.Material.iOS.Material.Init();
            XamEffects.iOS.Effects.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            _ = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);
            LoadApplication(Startup.Init(ConfigureServices));

            return base.FinishedLaunching(app, options);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ILocalizeService), typeof(LocalizeService));
            services.AddSingleton(typeof(IAppQuit), typeof(AppQuit));
            services.AddSingleton(typeof(IToast), typeof(Toast));
            services.AddSingleton(typeof(IKeyboard), typeof(Keyboard));
        }
    }
}
