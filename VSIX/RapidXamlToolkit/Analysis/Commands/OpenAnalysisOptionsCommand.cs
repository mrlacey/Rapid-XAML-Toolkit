// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class OpenAnalysisOptionsCommand : BaseCommand
    {
        public const int CommandId = 4137;

        private OpenAnalysisOptionsCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(RapidXamlPackage.AnalysisCommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static OpenAnalysisOptionsCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in OpenOptionsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new OpenAnalysisOptionsCommand(package, commandService, logger);
        }

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.Logger?.RecordFeatureUsage(nameof(OpenAnalysisOptionsCommand));

                this.AsyncPackage.ShowOptionPage(typeof(AnalysisOptionsGrid));
            }
            catch (Exception exc)
            {
                this.Logger?.RecordException(exc);
            }
        }
    }
}
