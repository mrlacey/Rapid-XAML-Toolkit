// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Logging;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class CopyToClipboardCommand : GetXamlFromCodeWindowBaseCommand
    {
        public const int CommandId = 4128;

        private CopyToClipboardCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static CopyToClipboardCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in CreateXamlStringCommand's constructor requires the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CopyToClipboardCommand(package, commandService, logger);
        }

        private async void Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.Logger?.RecordFeatureUsage(nameof(CopyToClipboardCommand));

                this.Logger?.RecordInfo("Attempting to copy XAML to clipboard.");
                var output = await this.GetXamlAsync(Instance.ServiceProvider);

                if (output != null && output.OutputType != AnalyzerOutputType.None)
                {
                    var message = output.Output;

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Clipboard.SetText(message);
                        await ShowStatusBarMessageAsync(Instance.ServiceProvider, $"Copied XAML for {output.OutputType}: {output.Name}");
                        this.Logger.RecordInfo($"Copied XAML for {output.OutputType}: {output.Name}");
                    }
                    else
                    {
                        this.Logger.RecordInfo("Nothing to copy to clipboard.");
                    }
                }
                else
                {
                    await ShowStatusBarMessageAsync(Instance.ServiceProvider, "No XAML copied.");
                    this.Logger.RecordInfo("No XAML copied.");
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
