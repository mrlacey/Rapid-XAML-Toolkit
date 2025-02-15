﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Logging
{
    public class RxtLogger : ILogger
    {
        public IVsActivityLog VsActivityLog { get; set; }

        public bool UseExtendedLogging { get; set; } = true;

        public static string TimeStampMessage(string message)
        {
            return $"[{DateTime.Now:HH:mm:ss.fff}]  {message}{Environment.NewLine}";
        }

        public void RecordError(string message, Dictionary<string, string> properties = null, bool force = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Activate the pane (bring to front) so errors are obvious
            if (force)
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
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            GeneralOutputPane.Instance.Write($"[{StringRes.RapidXamlToolkit}]  {message}");
            GeneralOutputPane.Instance.Activate();
        }

        public void RecordNotice(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            RxtOutputPane.Instance.Write(TimeStampMessage(message));
        }

        public void RecordInfo(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (this.UseExtendedLogging)
            {
                RxtOutputPane.Instance.Write(TimeStampMessage(message));
            }
        }

        public void RecordException(Exception exception)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.RecordError(StringRes.Error_ExceptionHeader);
            this.RecordError(StringRes.Error_ExceptionHeaderUnderline);
            this.RecordError(exception.Message);
            this.RecordError(exception.Source);
            this.RecordError(exception.StackTrace);

            this.WriteToActivityLog($"{exception.Message}{Environment.NewLine}{exception.Source}{Environment.NewLine}{exception.StackTrace}");
        }

        public void RecordFeatureUsage(string feature, bool quiet = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (quiet)
            {
                return;
            }

            // this logger doesn't need to do anything special with feature usage messages
            this.RecordInfo(StringRes.Info_FeatureUsage.WithParams(feature));
        }

        public void WriteToActivityLog(string message)
        {
            if (this.VsActivityLog != null)
            {
                this.VsActivityLog.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, StringRes.RapidXamlToolkit, message);
            }
        }
    }
}
