// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Configuration;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    public class SharedRapidXamlPackage
    {
        public const string TelemetryGuid = "c735dfc3-c416-4501-bc33-558e2aaad8c5";

        public static ILogger Logger { get; set; }

        public static async Task InitializeAsync(CancellationToken cancellationToken, AsyncPackage package)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await package.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                if (Logger == null)
                {
                    var rxtLogger = new RxtLogger();

                    var config = new RxtSettings();

                    var telemLogger = TelemetryAccessor.Create(rxtLogger, config.TelemetryKey);

                    Logger = new RxtLoggerWithTelemtry(rxtLogger, telemLogger);

                    var activityLog = await package.GetServiceAsync<SVsActivityLog, IVsActivityLog>();
                    rxtLogger.VsActivityLog = activityLog;
                }

                // The RxtOutputPane is used by all extensions
                // so using that as a way to tell if any extensions have initialized.
                // Only want the default info loading once.
                if (!RxtOutputPane.IsInitialized())
                {
                    Logger.RecordNotice(StringRes.Info_ProblemsInstructionsAndLink);
                    Logger.RecordNotice(string.Empty);
                }
            }
            catch (Exception exc)
            {
                Logger.RecordException(exc);
            }
        }
    }
}
