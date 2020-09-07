// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Telemetry;

namespace RapidXamlToolkit.Logging
{
    public class RxtLoggerWithTelemtry : ILogger
    {
        public RxtLoggerWithTelemtry(RxtLogger logger, TelemetryAccessor telem)
        {
            this.Logger = logger;
            this.Telem = telem;
        }

        public RxtLogger Logger { get; }

        public TelemetryAccessor Telem { get; }

        public bool UseExtendedLogging { get; set; }

        public void RecordError(string message, Dictionary<string, string> properties = null, bool force = false)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                this.Logger.RecordError(message);
                this.Telem.TrackEvent(message, properties);
            });
        }

        public void RecordGeneralError(string message)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger.RecordGeneralError(message);
            });
        }

        public void RecordException(Exception exception)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger.RecordException(exception);
                this.Telem.TrackException(exception);
            });
        }

        public void RecordFeatureUsage(string feature, bool quiet = false)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger.RecordFeatureUsage(feature, quiet);
                this.Telem.TrackEvent(feature);
            });
        }

        public void RecordNotice(string message)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger.RecordNotice(message);
            });
        }

        public void RecordInfo(string message)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger.RecordInfo(message);
            });
        }
    }
}
