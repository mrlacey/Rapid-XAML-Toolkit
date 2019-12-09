// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Options;
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
            if (force || this.IsExtendedLoggingEnabled())
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

            if (this.IsExtendedLoggingEnabled())
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

        private bool IsExtendedLoggingEnabled()
        {
            Settings settings;

            try
            {
                settings = CodeParserBase.GetSettings();

                if (settings != null)
                {
                    return settings.ExtendedOutputEnabled;
                }
                else
                {
                    // Writing this directly to avoid potential infinite loop
                    RxtOutputPane.Instance.Write(TimeStampMessage(StringRes.Info_UnableToDetermineIfExtendedOutputEnabled));
                }
            }
            catch (Exception exc)
            {
                // May be unable to get settings if package hasn't loaded yet.
                System.Diagnostics.Debug.WriteLine(exc.Message);
            }

            return true;
        }
    }
}
