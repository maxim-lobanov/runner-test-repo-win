using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AviaExplorer.Services.Utils.Analytics
{
    public interface IAnalyticsService
    {
        void TrackEvent(string name, Dictionary<string, string> keys = null, string additional = null);

        void TrackError(Exception ex, Dictionary<string, string> keys = null, string additional = null);

        Task ShowAndTrackErrorAsync(Exception ex, string message = null, string title = null, string curEvent = null);
    }
}