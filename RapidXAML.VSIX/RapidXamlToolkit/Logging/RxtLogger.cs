// <copyright file="RxtLogger.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit
{
    public class RxtLogger : ILogger
    {
        public void RecordError(string message)
        {
            // Activate the pane (bring to front) so errors are obvious
            if (AnalyzerBase.GetSettings().ExtendedOutputEnabled)
            {
                RxtOutputPane.Instance.Write(message);
                RxtOutputPane.Instance.Activate();
            }
            else
            {
                GeneralOutputPane.Instance.Write(message);
                GeneralOutputPane.Instance.Activate();
            }
        }

        public void RecordInfo(string message)
        {
            if (AnalyzerBase.GetSettings().ExtendedOutputEnabled)
            {
                RxtOutputPane.Instance.Write($"{message}{System.Environment.NewLine}");
            }
        }
    }
}
