using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Avia.RestClient;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Message;
using AviaExplorer.Services.Utils.Navigation;
using AviaExplorer.Services.Utils.Settings;
using AviaExplorer.Services.Utils.Shell;
using AviaExplorer.ViewModels.Avia;
using AviaExplorer.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Xamarin.Forms;

namespace AviaExplorer
{
    /// <summary>
    /// We create application and all deps throughout this class
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Use this method to initialize application
        /// </summary>
        /// <param name="nativeConfigureServices">Native services' configure callback</param>
        /// <returns>Application</returns>
        public static App Init(Action<IServiceCollection> nativeConfigureServices)
        {
            var host = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseDefaultServiceProvider((context, options) =>
                {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
                .ConfigureServices(x =>
                {
                    nativeConfigureServices(x);
                    ConfigureServices(x);
                })
                .RegisterRoutes()
                .Build();

            App.Services = host.Services;

            return App.Services.GetService<App>();
        }

        /// <summary>
        /// Crossplatform services configuration
        /// </summary>
        /// <param name="services">Services collection</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            #region Services
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IShellService, ShellService>();

            services.AddSingleton<IAviaInfoService, AviaInfoService>();
            services.AddSingleton<IRestClientProvider, RestClientProvider>();
            #endregion

            #region ViewModels
            services.AddSingleton<OriginSelectionViewModel>();
            services.AddSingleton<DirectionsViewModel>();
            services.AddSingleton<FlightDetailViewModel>();
            #endregion

            #region Application
            services.AddSingleton<App>();
            #endregion
        }

        //public static void ExtractSaveResource(string filename, string location)
        //{
        //    var a = Assembly.GetExecutingAssembly();
        //    using (var resFilestream = a.GetManifestResourceStream(filename))
        //    {
        //        if (resFilestream != null)
        //        {
        //            var full = Path.Combine(location, filename);

        //            using (var stream = File.Create(full))
        //            {
        //                resFilestream.CopyTo(stream);
        //            }
        //        }
        //    }
        //}
    }

    static class StartupExtensions
    {
        /// <summary>
        /// Registers routes for shell navigation
        /// </summary>
        /// <param name="host">Host abstraction</param>
        /// <returns>Host abstraction</returns>
        public static IHostBuilder RegisterRoutes(this IHostBuilder host)
        {
            Routing.RegisterRoute("directions", typeof(DirectionsPage));
            Routing.RegisterRoute("flights", typeof(FlightDetailPage));
            return host;
        }
    }
}