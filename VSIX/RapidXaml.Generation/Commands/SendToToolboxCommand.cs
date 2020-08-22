// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class SendToToolboxCommand : GetXamlFromCodeWindowBaseCommand
    {
        public const int CommandId = 4129;

        private SendToToolboxCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(RapidXamlGenerationPackage.GenerationCommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SendToToolboxCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in SendToToolboxCommand's constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new SendToToolboxCommand(package, commandService, logger);
        }

        private static async Task AddToToolboxAsync(string label, string actualText)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var tbs = await Instance.AsyncPackage.GetServiceAsync<IVsToolbox, IVsToolbox>();

            var itemInfo = new TBXITEMINFO[1];
            var tbItem = new OleDataObject();

            var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var bitmap = new System.Drawing.Bitmap(Path.Combine(executionPath, "Resources", "MarkupTag_16x.png"));

            itemInfo[0].hBmp = bitmap.GetHbitmap();
            itemInfo[0].bstrText = label;
            itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

            tbItem.SetText(actualText, TextDataFormat.Text);

            tbs?.AddItem(tbItem, itemInfo, StringRes.UI_ToolboxGroupHeader);
        }

#pragma warning disable VSTHRD100 // Avoid async void methods - Allowed as called from event handler.
        private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            System.Windows.Forms.Cursor previousCursor = System.Windows.Forms.Cursor.Current;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger?.RecordFeatureUsage(nameof(SendToToolboxCommand));

                this.Logger?.RecordInfo(StringRes.Info_AttemptingToAddToToolbox);
                var parserResult = await this.GetXamlAsync(Instance.AsyncPackage);

                if (parserResult != null && parserResult.OutputType != ParserOutputType.None)
                {
                    var label = $"{parserResult.OutputType}: {parserResult.Name}";

                    await AddToToolboxAsync(label, parserResult.Output);

                    await ShowStatusBarMessageAsync(Instance.AsyncPackage, StringRes.UI_AddedXamlToToolbox.WithParams(label));
                    this.Logger.RecordInfo(StringRes.UI_AddedXamlToToolbox.WithParams(label));
                }
                else
                {
                    await ShowStatusBarMessageAsync(Instance.AsyncPackage, StringRes.UI_NothingAddedToToolbox);
                    this.Logger.RecordInfo(StringRes.UI_NothingAddedToToolbox);
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
