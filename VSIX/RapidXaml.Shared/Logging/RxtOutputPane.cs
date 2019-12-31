// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Logging
{
    public class RxtOutputPane : IOutputPane
    {
        private static Guid rxtPaneGuid = new Guid("32C5FA5D-E91C-4113-8B22-3396D748D429");

        private static RxtOutputPane instance;

        private readonly IVsOutputWindowPane rxtPane;

        private RxtOutputPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outWindow)
            {
                outWindow.GetPane(ref rxtPaneGuid, out this.rxtPane);

                if (this.rxtPane == null)
                {
                    outWindow.CreatePane(ref rxtPaneGuid, StringRes.UI_RxtOutputPaneTitle, 1, 0);
                    outWindow.GetPane(ref rxtPaneGuid, out this.rxtPane);
                }
            }
        }

        public static RxtOutputPane Instance => instance ?? (instance = new RxtOutputPane());

        public static bool IsInitialized()
        {
            if (ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outWindow)
            {
                outWindow.GetPane(ref rxtPaneGuid, out IVsOutputWindowPane pane);

                return pane != null;
            }

            return false;
        }

        public void Write(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.rxtPane.OutputString(message);
        }

        public void Activate()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.rxtPane.Activate();
        }
    }
}
