// <copyright file="SetDatacontextCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    internal sealed class SetDatacontextCommand
    {
        public const int CommandId = 4132;

        public static readonly Guid CommandSet = new Guid("8c20aab1-50b0-4523-8d9d-24d512fa8154");

        private readonly AsyncPackage package;
        private readonly ILogger logger;

        private SetDatacontextCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            this.logger = logger;

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static SetDatacontextCommand Instance
        {
            get;
            private set;
        }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in SetDatacontextCommand's constructor requires
            // the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SetDatacontextCommand(package, commandService, logger);

            AnalyzerBase.ServiceProvider = (IServiceProvider)Instance.ServiceProvider;
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (sender is OleMenuCommand menuCmd)
                {
                    menuCmd.Visible = menuCmd.Enabled = false;

                    if (AnalyzerBase.GetSettings().IsActiveProfileSet)
                    {
                        // TODO: other logic here about whether this should be shown - need to review the perf impact of doing this. - better to show when shouldn't than make VS slow
                        // find the file where it should be set
                        // only show if not already set where it should be
                        // Get active document
                        // - if XAML & should be set in XAML
                        // - if XAML & should be set in CB get CB doc
                        // - if CB & should be set in CB
                        // - if CB & should be set in XAML get XAML doc
                        menuCmd.Visible = menuCmd.Enabled = true;
                    }
                }
            }
            catch (Exception exc)
            {
                this.logger.RecordException(exc);
                throw;
            }
        }

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                // TODO: Add actual logic for setting the datacontext
                // If in xamlfile
                //  - if should set here then modify current document
                //  - If should set in CB, modify that file then open it unsaved
                // if in code behind
                //  - if should set here then modify current document
                //  - If should set in XAML, modify that file then open it unsaved
                ThreadHelper.ThrowIfNotOnUIThread();
                string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
                string title = "SetDatacontextCommand";

                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            catch (Exception exc)
            {
                this.logger.RecordException(exc);
                throw;
            }
        }
    }
}
