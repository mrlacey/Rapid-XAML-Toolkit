// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class FeedbackCommand : BaseCommand
    {
        public const int CommandId = 4135;

        public FeedbackCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static FeedbackCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in FeedbackCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new FeedbackCommand(package, commandService, logger);
        }

#pragma warning disable VSTHRD100 // Avoid async void methods - ok as an event handler
        private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger?.RecordFeatureUsage(nameof(FeedbackCommand));

                System.Diagnostics.Process.Start("https://github.com/mrlacey/Rapid-XAML-Toolkit/issues/new/choose");
            }
            catch (Exception exc)
            {
                this.Logger?.RecordException(exc);
            }
        }
    }
}
