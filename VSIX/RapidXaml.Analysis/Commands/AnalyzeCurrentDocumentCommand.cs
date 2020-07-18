// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class AnalyzeCurrentDocumentCommand : BaseCommand
    {
        public const int CommandId = 4136;

        private AnalyzeCurrentDocumentCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(RapidXamlAnalysisPackage.AnalysisCommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static AnalyzeCurrentDocumentCommand Instance
        {
            get;
            private set;
        }

        public static AsyncPackage Package
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in FeedbackCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            Package = package;
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AnalyzeCurrentDocumentCommand(package, commandService, logger);
        }

#pragma warning disable VSTHRD100 // Avoid async void methods - ok as an event handler
        private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            System.Windows.Forms.Cursor previousCursor = System.Windows.Forms.Cursor.Current;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger?.RecordFeatureUsage(nameof(AnalyzeCurrentDocumentCommand));

                var dte = await Instance.ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;
                var vs = new VisualStudioAbstraction(this.Logger, this.ServiceProvider, dte);

                var filePath = vs.GetActiveDocumentFilePath();

                // Ensure that the open document has been saved so get latest version
                var rdt = new RunningDocumentTable(Package);
                rdt.SaveFileIfDirty(filePath);

                RapidXamlDocumentCache.Invalidate(filePath);
                RapidXamlDocumentCache.TryUpdate(filePath);
            }
            catch (Exception exc)
            {
                this.Logger?.RecordException(exc);
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = previousCursor;
            }
        }
    }
}
