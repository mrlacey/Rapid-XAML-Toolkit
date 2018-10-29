// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class InsertGridRowDefinitionCommand : BaseCommand
    {
        public const int CommandId = 4133;

        private InsertGridRowDefinitionCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static InsertGridRowDefinitionCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in the constructor requires the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new InsertGridRowDefinitionCommand(package, commandService, logger);
        }

        private async void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MenuItem_BeforeQueryStatus");
            try
            {
                if (sender is OleMenuCommand menuCmd)
                {
                    var dte = await Instance.ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;

                    var logic = new InsertGridRowDefinitionCommandLogic(this.Logger, new VisualStudioAbstraction(dte));

                    var showCommandButton = logic.ShouldEnableCommand();

                    // Changed text isn't shown until subsequent display and so wrong number may be shown
                    ////if (showCommandButton)
                    ////{
                    ////    var rowNumber = logic.GetRowNumber();
                    ////    menuCmd.Text = $"Insert new row {rowNumber} definition"; // Will need localizing if/when get this working
                    ////}

                    menuCmd.Visible = menuCmd.Enabled = showCommandButton;
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
                throw;
            }
        }

        private async void Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.Logger?.RecordFeatureUsage(nameof(InsertGridRowDefinitionCommand));

                var dte = await Instance.ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;
                var vs = new VisualStudioAbstraction(dte);

                var logic = new InsertGridRowDefinitionCommandLogic(this.Logger, vs);

                var replacements = logic.GetReplacements();
                var (start, end, exclusions) = logic.GetGridBoundary();
                var (newDefinition, newDefPos) = logic.GetDefinitionAtCursor();

                vs.StartSingleUndoOperation(StringRes.Info_UndoContextIndertRowDef);
                try
                {
                    vs.ReplaceInActiveDoc(replacements, start, end, exclusions);
                    vs.InsertIntoActiveDocumentOnNextLine(newDefinition, newDefPos);
                }
                finally
                {
                    vs.EndSingleUndoOperation();
                }
            }
            catch (Exception exc)
            {
                this.Logger?.RecordException(exc);
                throw;
            }
        }
    }
}
