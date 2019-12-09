// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Reflection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.Telemetry;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Telemetry
{
    public class TelemetryAccessor : IDisposable
    {
        private readonly ILogger logger;

        private TelemetryAccessor(ILogger logger)
        {
            this.logger = logger;
        }

        public bool IsEnabled { get; private set; }

        private TelemetryClient Client { get; set; }

        public static TelemetryAccessor Create(ILogger logger, string key)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            TelemetryAccessor instance = null;

            try
            {
                instance = new TelemetryAccessor(logger);

                if (VsTelemetryIsOptedIn(logger) && !string.IsNullOrWhiteSpace(key))
                {
                    instance.Client = new TelemetryClient()
                    {
                        InstrumentationKey = key,
                    };

                    TelemetryConfiguration.Active.InstrumentationKey = key;

                    SetContextData(instance.Client);

                    instance.Client.TrackEvent(TelemetryEvents.SessionStart);

                    instance.IsEnabled = true;
#if DEBUG
                    TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif
                }
                else
                {
                    instance.IsEnabled = false;
                    TelemetryConfiguration.Active.DisableTelemetry = true;
                }
            }
            catch (Exception ex)
            {
                if (instance != null)
                {
                    instance.IsEnabled = false;
                }

                TelemetryConfiguration.Active.DisableTelemetry = true;

                logger.RecordException(ex);
            }

            return instance;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Flush()
        {
            try
            {
                this.Client?.Flush();
            }
            catch (Exception ex)
            {
                this.logger?.RecordException(ex);
            }
        }

        public void TrackException(Exception exc)
        {
            if (this.IsEnabled && this.Client.IsEnabled())
            {
                this.Client.TrackException(exc);
                this.Flush();
            }
        }

        public void TrackEvent(string eventName)
        {
            if (this.IsEnabled && this.Client.IsEnabled())
            {
                this.Client.TrackEvent(eventName);
                this.Flush();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                this.Flush();

                // Allow time for flushing - AppInsights recommendation
                System.Threading.Thread.Sleep(1000);
            }

            // free native resources if any
        }

        private static void SetContextData(TelemetryClient client)
        {
            // No PII tracked
            string userToTrack = Guid.NewGuid().ToString();
            string machineToTrack = Guid.NewGuid().ToString();
            string sessionId = Guid.NewGuid().ToString();

            client.Context.User.Id = userToTrack;
            client.Context.User.AccountId = userToTrack;
            client.Context.User.AuthenticatedUserId = userToTrack;

            client.Context.Device.Id = machineToTrack;
            client.Context.Device.OperatingSystem = Environment.OSVersion.VersionString;

            client.Context.Cloud.RoleInstance = TelemetryProperties.RoleInstanceName;
            client.Context.Cloud.RoleName = TelemetryProperties.RoleInstanceName;

            client.Context.Session.Id = sessionId;
            client.Context.Component.Version = CoreDetails.GetVersion();

            client.Context.GlobalProperties.Add(TelemetryProperties.VisualStudioEdition, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.SkuName"));
            client.Context.GlobalProperties.Add(TelemetryProperties.VisualStudioExeVersion, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.ExeVersion"));
            client.Context.GlobalProperties.Add(TelemetryProperties.VisualStudioCulture, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.Locale.ProductLocaleName"));
            client.Context.GlobalProperties.Add(TelemetryProperties.VisualStudioManifestId, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.ManifestId"));
        }

        private static bool VsTelemetryIsOptedIn(ILogger logger)
        {
            var isOptedIn = false;
            try
            {
                // If this fails to load then can't find out if telemetry is enabled so assume it isn't
                Assembly.Load(new AssemblyName("Microsoft.VisualStudio.Telemetry"));

                if (TelemetryService.DefaultSession != null)
                {
                    isOptedIn = TelemetryService.DefaultSession.IsOptedIn;
                }
            }
            catch (Exception)
            {
                logger.RecordInfo(StringRes.Info_UnableToAccessTelemetry);
                isOptedIn = false;
            }

            return isOptedIn;
        }
    }
}
