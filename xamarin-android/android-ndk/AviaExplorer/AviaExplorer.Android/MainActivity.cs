using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AviaExplorer.Droid.Implementations;
using AviaExplorer.Services.Interfaces;
using FFImageLoading.Forms.Platform;
using Microsoft.Extensions.DependencyInjection;
using AGlide = Android.Glide;

namespace AviaExplorer.Droid
{
    [Activity(Label = "AviaExplorer", Icon = "@mipmap/icon", Theme = "@style/MainTheme",
        MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int RequestLocationId = 0;

        private readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                    RequestPermissions(LocationPermissions, RequestLocationId);
            }

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            AGlide.Forms.Init(this);
            XamEffects.Droid.Effects.Init();
            XF.Material.Droid.Material.Init(this, savedInstanceState);
            CachedImageRenderer.Init(true);
            _ = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);

            LoadApplication(Startup.Init(ConfigureServices));
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ILocalizeService), typeof(LocalizeService));
            services.AddSingleton(typeof(IAppQuit), typeof(AppQuit));
            services.AddSingleton(typeof(IToast), typeof(Toast));
            services.AddSingleton(typeof(IKeyboard), typeof(Keyboard));
        }

        public override void OnBackPressed() =>
            XF.Material.Droid.Material.HandleBackButton(base.OnBackPressed);

        public override void OnRequestPermissionsResult(int requestCode, 
            string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}