// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace RapidXamlToolkit.Logging
{
    public class GeneralOutputPane : IOutputPane
    {
        private static GeneralOutputPane instance;

        private readonly IVsOutputWindowPane generalPane;

        private GeneralOutputPane()
        {
            var outWindow = ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;

            if (outWindow != null && (ErrorHandler.Failed(outWindow.GetPane(ref generalPaneGuid, out this.generalPane)) || this.generalPane == null))
            {
                if (ErrorHandler.Failed(outWindow.CreatePane(ref generalPaneGuid, "General", 1, 0)))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create the Output window pane.");
                    return;
                }

                if (ErrorHandler.Failed(outWindow.GetPane(ref generalPaneGuid, out this.generalPane)) || (this.generalPane == null))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get the Output window pane.");
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
