// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXamlToolkit.Logging;

namespace RapidXaml.AnalysisExe
{
    internal class AnalysisExeLogger : ILogger
    {
        public bool UseExtendedLogging { get; set; } = true;

        public void RecordError(string message, bool force = false)
        {
            Console.WriteLine($"AEL-Error: {message}");
        }

        public void RecordException(Exception exception)
        {
            Console.WriteLine($"AEL-Exception: {exception.Message}");
        }

        public void RecordFeatureUsage(string feature, bool quiet = false)
        {
        }

        public void RecordGeneralError(string message)
        {
            Console.WriteLine($"AEL-GeneralError: {message}");
        }

        public void RecordInfo(string message)
        {
        }

        public void RecordNotice(string message)
        {
        }
    }
}
