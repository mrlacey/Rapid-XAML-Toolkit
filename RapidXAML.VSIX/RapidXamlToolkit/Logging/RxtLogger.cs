// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXamlToolkit.Analyzers;

namespace RapidXamlToolkit.Logging
{
    public class RxtLogger : ILogger
    {
        public static string TimeStampMessage(string message)
        {
            return $"[{DateTime.Now:HH:mm:ss.fff}]  {message}{Environment.NewLine}";
        }

        public void RecordError(string message)
        {
            // Activate the pane (bring to front) so errors are obvious
            if (AnalyzerBase.GetSettings().ExtendedOutputEnabled)
            {
                RxtOutputPane.Instance.Write(TimeStampMessage(message));
                RxtOutputPane.Instance.Activate();
            }
            else
            {
                GeneralOutputPane.Instance.Write(TimeStampMessage(message));
                GeneralOutputPane.Instance.Activate();
            }
        }

        public void RecordInfo(string message)
        {
            if (AnalyzerBase.GetSettings().ExtendedOutputEnabled)
            {
                RxtOutputPane.Instance.Write(TimeStampMessage(message));
            }
        }

        public void RecordException(Exception exception)
        {
            this.RecordError("Exception");
            this.RecordError("=========");
            this.RecordError(exception.Message);
            this.RecordError(exception.Source);
            this.RecordError(exception.StackTrace);
        }

        public void RecordFeatureUsage(string feature)
        {
            // this logger doesn't need to do anything special with feature usage messages
            this.RecordInfo(feature);
        }
    }
}
