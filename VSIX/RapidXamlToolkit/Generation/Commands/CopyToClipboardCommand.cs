// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
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

            var menuCommandId = new CommandID(PackageGuids.guidRapidXamlGenerationPackageCmdSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
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
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new CopyToClipboardCommand(package, commandService, logger);
        }

#pragma warning disable VSTHRD100 // Avoid async void methods - Allowed as called from event handler.
        private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            Cursor previousCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger?.RecordFeatureUsage(nameof(CopyToClipboardCommand));

                this.Logger?.RecordInfo(StringRes.Info_AttemptingToCopy);
                var output = await this.GetXamlAsync(Instance.AsyncPackage);

                if (output != null && output.OutputType != ParserOutputType.None)
                {
                    var message = output.Output;

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Clipboard.SetText(message);
                        await ShowStatusBarMessageAsync(Instance.AsyncPackage, StringRes.Info_CopiedXaml.WithParams(output.OutputType, output.Name));
                        this.Logger?.RecordInfo(StringRes.Info_CopiedXaml.WithParams(output.OutputType, output.Name));
                    }
                    else
                    {
                        this.Logger?.RecordInfo(StringRes.Info_NothingToCopy);
                    }
                }
                else
                {
                    await ShowStatusBarMessageAsync(Instance.AsyncPackage, StringRes.Info_NoXamlCopied);
                    this.Logger?.RecordInfo(StringRes.Info_NoXamlCopied);
                }
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
