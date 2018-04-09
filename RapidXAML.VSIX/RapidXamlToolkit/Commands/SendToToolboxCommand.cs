// <copyright file="SendToToolboxCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

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

        public static readonly Guid CommandSet = new Guid("8c20aab1-50b0-4523-8d9d-24d512fa8154");

        private readonly AsyncPackage package;

        private SendToToolboxCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SendToToolboxCommand Instance
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

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in CreateXamlStringCommand's constructor requires the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SendToToolboxCommand(package, commandService);

            AnalyzerBase.ServiceProvider = (IServiceProvider)Instance.ServiceProvider;
        }

        private static void AddToToolbox(string label, string actualText)
        {
            IVsToolbox tbs = Instance.ServiceProvider.GetServiceAsync(typeof(IVsToolbox)).Result as IVsToolbox;

            TBXITEMINFO[] itemInfo = new TBXITEMINFO[1];
            OleDataObject tbItem = new OleDataObject();

            itemInfo[0].bstrText = label; // sumary
            itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

            tbItem.SetText(actualText, TextDataFormat.Text);

            tbs.AddItem(tbItem, itemInfo, "Rapid XAML");
        }

        private void Execute(object sender, EventArgs e)
        {
            var analyzerResult = this.GetXaml(Instance.ServiceProvider);

            if (analyzerResult != null && analyzerResult.OutputType != AnalyzerOutputType.None)
            {
                var label = $"{analyzerResult.OutputType}: {analyzerResult.Name}";

                AddToToolbox(label, analyzerResult.Output);

                ShowStatusBarMessage(Instance.ServiceProvider, $"Copied XAML for {label}");
            }
            else
            {
                ShowStatusBarMessage(Instance.ServiceProvider, "No XAML copied.");
            }
        }
    }
}
