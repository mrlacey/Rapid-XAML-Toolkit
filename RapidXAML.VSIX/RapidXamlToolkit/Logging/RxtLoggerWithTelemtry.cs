// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
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

        public void RecordError(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.Logger.RecordError(message);
        }

        public void RecordException(Exception exception)
        {
            this.Logger.RecordException(exception);
            this.Telem.TrackException(exception);
        }

        public void RecordFeatureUsage(string feature)
        {
            this.Logger.RecordFeatureUsage(feature);
            this.Telem.TrackEvent(feature);
        }

        public void RecordInfo(string message)
        {
            this.Logger.RecordInfo(message);
        }
    }
}
