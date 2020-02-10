﻿using System;
using System.Collections.Generic;
using Android.OS;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using PE.Framework.Droid.AppVersion;

namespace PE.Plugins.Analytics.AppInsights.Droid
{
    public class DroidAnalyticsService : AnalyticsService, IAnalyticsService
    {
        private TelemetryClient _Telemetry;

        public DroidAnalyticsService(AIConfiguration configuration)
        {
            _Telemetry = new TelemetryClient { InstrumentationKey = configuration.InstrumentationKey };

            var version = new AppVersionImpl();

#if DEBUG
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif
            _Telemetry.Context.Device.OperatingSystem = $"Android {Build.VERSION.SdkInt} ({Build.VERSION.Release})";
            _Telemetry.Context.GlobalProperties.Add("&aid", configuration.AppId);
            _Telemetry.Context.GlobalProperties.Add("&an", configuration.AppName);
            _Telemetry.Context.GlobalProperties.Add("&av", version.Version);
        }

        public override void RecordException(string exDescription, bool exFatal)
        {
            _Telemetry.TrackException(new Exception(exDescription), new Dictionary<string, string> { { "description", exDescription }, { "fatal", $"{exFatal}" } });
        }

        public override void RecordException(Exception exception, string description)
        {
            _Telemetry.TrackException(exception, new Dictionary<string, string> { { "description", description } });
        }

        public override void SetUserId(string userId)
        {
            _Telemetry.Context.GlobalProperties.Add("&uid", userId ?? string.Empty);
        }

        public override void StartAnalytics()
        {
            // NOP
        }

        public override void TrackEvent(string category, string action)
        {
            _Telemetry.TrackEvent(category, new Dictionary<string, string> { { "action", action } });
        }

        public override void TrackEvent(string category, string action, string Label)
        {
            _Telemetry.TrackEvent(category, new Dictionary<string, string> { { "action", action }, { "label", Label } });
        }
    }
}
