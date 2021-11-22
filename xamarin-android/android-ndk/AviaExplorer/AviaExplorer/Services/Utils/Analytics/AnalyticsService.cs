using AviaExplorer.Services.Utils.Message;
using AviaExplorer.Services.Utils.Settings;
using Microsoft.AppCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AviaExplorer.Services.Utils.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ISettingsService _settings;
        private readonly IMessageService _message;

        public AnalyticsService(ISettingsService settings,
                                IMessageService message)
        {
            _settings = settings;
            _message = message;

            Init();
        }

        private void Init() =>
            AppCenter.Start(_settings.AppCenterAndroidKey + _settings.AppCenteriOSKey,
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));

        public void TrackError(Exception ex, Dictionary<string, string> keys = null, string additional = null)
        {
            var dataDict = new Dictionary<string, string>();
            dataDict.Add("exception message", ex.Message);
            if (keys != null)
                dataDict = dataDict.Concat(keys).ToDictionary(x => x.Key, x => x.Value);
            if (!string.IsNullOrEmpty(additional))
                dataDict.Add("Additional Info", additional);
            Microsoft.AppCenter.Crashes.Crashes.TrackError(ex, dataDict);
        }

        public void TrackEvent(string name, Dictionary<string, string> keys = null, string additional = null)
        {
            var dataDict = new Dictionary<string, string>();
            if (keys != null)
                dataDict = dataDict.Concat(keys).ToDictionary(x => x.Key, x => x.Value);
            if (!string.IsNullOrEmpty(additional))
                dataDict.Add("Additional Info", additional);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(name, dataDict);
        }

        public Task ShowAndTrackErrorAsync(
            Exception ex,
            string message = null,
            string title = null,
            string currentEvent = null)
        {
            if (currentEvent != null) TrackEvent(currentEvent);
            TrackError(ex);
            return _message.DisplayInfoAsync(message);
        }
    }
}