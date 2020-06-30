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
            Console.WriteLine($"Logger-Error: {message}");
        }

        public void RecordException(Exception exception)
        {
            Console.WriteLine($"Logger-Exception: {exception.Message}");
        }

        public void RecordFeatureUsage(string feature, bool quiet = false)
        {
            Console.WriteLine($"Logger-RecordFeatureUsage: {feature}");
        }

        public void RecordGeneralError(string message)
        {
            Console.WriteLine($"Logger-GeneralError: {message}");
        }

        public void RecordInfo(string message)
        {
            Console.WriteLine($"Logger-Info: {message}");
        }

        public void RecordNotice(string message)
        {
            Console.WriteLine($"Logger-Notice: {message}");
        }
    }
}
