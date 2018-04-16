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

            outWindow.GetPane(ref generalPaneGuid, out this.generalPane);
        }

        public static GeneralOutputPane Instance => instance ?? (instance = new GeneralOutputPane());

        public void Activate()
        {
            this.generalPane.Activate();
        }

        public void Write(string message)
        {
            this.generalPane.OutputString($"{message}{Environment.NewLine}");
        }
    }
}
