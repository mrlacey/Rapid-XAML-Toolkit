// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.Tests
{
    public class DefaultTestLogger : ILogger
    {
        public bool UseExtendedLogging { get; set; }

        public static DefaultTestLogger Create()
        {
            return new DefaultTestLogger();
        }

        public void RecordError(string message, Dictionary<string, string> properties = null, bool force = false)
        {
        }

        public void RecordGeneralError(string message)
        {
        }

        public void RecordException(Exception exception)
        {
        }

        public void RecordFeatureUsage(string feature, bool quiet = false)
        {
        }

        public void RecordInfo(string message)
        {
        }

        public void RecordNotice(string message)
        {
        }
    }
}
