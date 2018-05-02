// <copyright file="TelemetryAccessor.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.Telemetry;

namespace RapidXamlToolkit.Telemetry
{
    public class TelemetryAccessor : IDisposable
    {
        private static TelemetryAccessor instance;
        private ILogger logger;

        public TelemetryAccessor(ILogger logger)
        {
            this.logger = logger;
        }

        public static TelemetryAccessor Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new NullReferenceException("TelemetryService must be initialized before use");
                }

                return instance;
            }
        }

        public bool IsEnabled { get; private set; }

        private TelemetryClient Client { get; set; }

        public static void InitializeInstance(ILogger logger, string key)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

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
                   // TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
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
                instance.IsEnabled = false;
                TelemetryConfiguration.Active.DisableTelemetry = true;

                logger.RecordException(ex);
            }
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
                if (instance != null)
                {
                    instance.Client?.Flush();

                    instance = null;
                }
            }
            catch (Exception ex)
            {
                instance?.logger?.RecordException(ex);
            }
        }

        public void TrackException(Exception exc)
        {
            if (this.IsEnabled && this.Client.IsEnabled())
            {
                this.Client.TrackException(exc);
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

                // Allow time for flushing - APPInsights recommendation
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

            client.Context.Properties.Add(TelemetryProperties.VisualStudioEdition, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.SkuName"));
            client.Context.Properties.Add(TelemetryProperties.VisualStudioExeVersion, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.ExeVersion"));
            client.Context.Properties.Add(TelemetryProperties.VisualStudioCulture, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.Locale.ProductLocaleName"));
            client.Context.Properties.Add(TelemetryProperties.VisualStudioManifestId, TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.ManifestId"));
        }

        private static bool VsTelemetryIsOptedIn(ILogger logger)
        {
            var isOptedIn = false;
            try
            {
                // If this fails to load the can't find out if telemtry is enabled so assume it isn't
                Assembly.Load(new AssemblyName("Microsoft.VisualStudio.Telemetry"));

                if (TelemetryService.DefaultSession != null)
                {
                    isOptedIn = TelemetryService.DefaultSession.IsOptedIn;
                }
            }
            catch (Exception)
            {
                logger.RecordInfo($"Unable to load the assembly 'Microsoft.VisualStudio.Telemetry' so telemetry will not be enabled.");
                isOptedIn = false;
            }

            return isOptedIn;
        }
    }
}
