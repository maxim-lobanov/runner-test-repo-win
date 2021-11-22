using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Navigation;
using System;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace AviaExplorer
{
    public partial class App
    {
        #region Fields
        private ILanguageService _language;
        private INavigationService _navigation;
        private IAnalyticsService _analytics;
        #endregion

        #region Properties
        public static IServiceProvider Services { get; set; }
        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();
            Current
                .On<Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            InitApp();
        }
        #endregion

        #region Methods
        private void InitApp()
        {
            XF.Material.Forms.Material.Init(this);

            _language = (ILanguageService)Services.GetService(typeof(ILanguageService));
            _navigation = (INavigationService)Services.GetService(typeof(INavigationService));
            _analytics = (IAnalyticsService)Services.GetService(typeof(IAnalyticsService));
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            _language.DetermineAndSetLanguage();
            _navigation.DetermineAndSetMainPage();
            _analytics.TrackEvent("App started.");
        }
        #endregion
    }
}