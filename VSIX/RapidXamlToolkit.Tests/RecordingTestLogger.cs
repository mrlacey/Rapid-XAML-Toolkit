// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.Tests
{
    public class RecordingTestLogger : ILogger
    {
        public List<string> Errors { get; set; } = new List<string>();

        public List<Exception> Exceptions { get; set; } = new List<Exception>();

        public List<string> UsedFeatures { get; set; } = new List<string>();

        public List<string> GeneralErrors { get; set; } = new List<string>();

        public List<string> Info { get; set; } = new List<string>();

        public void RecordError(string message, bool force = false)
        {
            this.Errors.Add(message);
        }

        public void RecordException(Exception exception)
        {
            this.Exceptions.Add(exception);
        }

        public void RecordFeatureUsage(string feature)
        {
            this.UsedFeatures.Add(feature);
        }

        public void RecordGeneralError(string message)
        {
            this.GeneralErrors.Add(message);
        }

        public void RecordInfo(string message)
        {
            this.Info.Add(message);
        }
    }
}
