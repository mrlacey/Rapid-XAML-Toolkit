// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXamlToolkit.Logging;

namespace RapidXaml
{
    public class EmptyLogger : ILogger
    {
        public bool UseExtendedLogging { get; set; }

        public static EmptyLogger Create()
        {
            return new EmptyLogger();
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
