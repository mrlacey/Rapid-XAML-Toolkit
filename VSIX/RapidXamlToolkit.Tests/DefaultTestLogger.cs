// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
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

        public void RecordError(string message, bool force = false)
        {
        }

        public void RecordGeneralError(string message)
        {
        }

        public void RecordException(Exception exception)
        {
        }

        public void RecordFeatureUsage(string feature)
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
