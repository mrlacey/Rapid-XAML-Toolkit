// <copyright file="RxtLogger.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace RapidXamlToolkit
{
    public class RxtLogger : ILogger
    {
        public static string TimeStampMessage(string message)
        {
            return $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]  {message}{Environment.NewLine}";
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
    }
}
