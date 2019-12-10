// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Configuration;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Telemetry;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    public class RapidXamlPackage : AsyncPackage
    {
        public const string TelemetryGuid = "c735dfc3-c416-4501-bc33-558e2aaad8c5";

        public static ILogger Logger { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var rxtLogger = new RxtLogger();

            var config = new RxtSettings();

            var telemLogger = TelemetryAccessor.Create(rxtLogger, config.TelemetryKey);

            Logger = new RxtLoggerWithTelemtry(rxtLogger, telemLogger);

            try
            {
                var activityLog = await this.GetServiceAsync(typeof(SVsActivityLog)) as IVsActivityLog;
                rxtLogger.VsActivityLog = activityLog;

                // Set the ServiceProvider of CodeParserBase as it's needed to get settings
                CodeParserBase.ServiceProvider = this;
             }
            catch (Exception exc)
            {
                Logger.RecordException(exc);
            }
        }
    }
}
