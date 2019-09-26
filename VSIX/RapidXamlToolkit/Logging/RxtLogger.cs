// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Logging
{
    public class RxtLogger : ILogger
    {
        public IVsActivityLog VsActivityLog { get; set; }

        public static string TimeStampMessage(string message)
        {
            return $"[{DateTime.Now:HH:mm:ss.fff}]  {message}{Environment.NewLine}";
        }

        public void RecordError(string message, bool force = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Activate the pane (bring to front) so errors are obvious
            if (force || CodeParserBase.GetSettings().ExtendedOutputEnabled)
            {
                RxtOutputPane.Instance.Write(TimeStampMessage(message));
                RxtOutputPane.Instance.Activate();
            }
            else
            {
                this.RecordGeneralError(message);
            }
        }

        public void RecordGeneralError(string message)
        {
            GeneralOutputPane.Instance.Write($"[{StringRes.VSIX__LocalizedName}]  {message}{Environment.NewLine}{Environment.NewLine}");
            GeneralOutputPane.Instance.Activate();
        }

        public void RecordInfo(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (CodeParserBase.GetSettings().ExtendedOutputEnabled)
            {
                RxtOutputPane.Instance.Write(TimeStampMessage(message));
            }
        }

        public void RecordException(Exception exception)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.RecordError("Exception");
            this.RecordError("=========");
            this.RecordError(exception.Message);
            this.RecordError(exception.Source);
            this.RecordError(exception.StackTrace);

            this.WriteToActivityLog($"{exception.Message}{Environment.NewLine}{exception.Source}{Environment.NewLine}{exception.StackTrace}");
        }

        public void RecordFeatureUsage(string feature)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // this logger doesn't need to do anything special with feature usage messages
            this.RecordInfo(StringRes.Info_FeatureUsage.WithParams(feature));
        }

        public void WriteToActivityLog(string message)
        {
            if (this.VsActivityLog != null)
            {
                this.VsActivityLog.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, StringRes.VSIX__LocalizedName, message);
            }
        }
    }
}
