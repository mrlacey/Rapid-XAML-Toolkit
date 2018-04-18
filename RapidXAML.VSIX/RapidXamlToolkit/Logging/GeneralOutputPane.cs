// <copyright file="GeneralOutputPane.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace RapidXamlToolkit
{
    public class GeneralOutputPane : IOutputPane
    {
        private static GeneralOutputPane instance = null;

        private IVsOutputWindowPane generalPane = null;

        private GeneralOutputPane()
        {
            IVsOutputWindow outWindow = ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;

            if (ErrorHandler.Failed(outWindow.GetPane(ref generalPaneGuid, out this.generalPane)) || (this.generalPane == null))
            {
                if (ErrorHandler.Failed(outWindow.CreatePane(ref generalPaneGuid, "General", 1, 0)))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create the Output window pane.");
                    return;
                }

                if (ErrorHandler.Failed(outWindow.GetPane(ref generalPaneGuid, out this.generalPane)) || (this.generalPane == null))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get the Output window pane.");
                    return;
                }
            }
        }

        public static GeneralOutputPane Instance => instance ?? (instance = new GeneralOutputPane());

        public void Activate()
        {
            this.generalPane?.Activate();
        }

        public void Write(string message)
        {
            this.generalPane?.OutputString($"{message}{Environment.NewLine}");
        }
    }
}
