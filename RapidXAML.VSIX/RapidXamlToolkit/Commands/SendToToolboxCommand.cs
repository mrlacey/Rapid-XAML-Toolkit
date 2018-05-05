// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    internal sealed class SendToToolboxCommand : GetXamlFromCodeWindowBaseCommand
    {
        public const int CommandId = 4129;

        private SendToToolboxCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static SendToToolboxCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in CreateXamlStringCommand's constructor requires the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SendToToolboxCommand(package, commandService, logger);
        }

        private static void AddToToolbox(string label, string actualText)
        {
            IVsToolbox tbs = Instance.ServiceProvider.GetServiceAsync(typeof(IVsToolbox)).Result as IVsToolbox;

            TBXITEMINFO[] itemInfo = new TBXITEMINFO[1];
            OleDataObject tbItem = new OleDataObject();

            var bitmap = new System.Drawing.Bitmap("./Resources/MarkupTag_16x.png");

            itemInfo[0].hBmp = bitmap.GetHbitmap();
            itemInfo[0].bstrText = label;
            itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

            tbItem.SetText(actualText, TextDataFormat.Text);

            tbs.AddItem(tbItem, itemInfo, "Rapid XAML");
        }

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.Logger?.RecordFeatureUsage(nameof(SendToToolboxCommand));

                this.Logger.RecordInfo("Attempting to add XAML to the Toolbox.");
                var analyzerResult = this.GetXaml(Instance.ServiceProvider);

                if (analyzerResult != null && analyzerResult.OutputType != AnalyzerOutputType.None)
                {
                    var label = $"{analyzerResult.OutputType}: {analyzerResult.Name}";

                    AddToToolbox(label, analyzerResult.Output);

                    ShowStatusBarMessage(Instance.ServiceProvider, $"Added XAML to toolbox for {label}");
                    this.Logger.RecordInfo($"Added XAML to toolbox for {label}");
                }
                else
                {
                    ShowStatusBarMessage(Instance.ServiceProvider, "No XAML added to toolbox.");
                    this.Logger.RecordInfo("No XAML added to toolbox.");
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
                throw;
            }
        }
    }
}
