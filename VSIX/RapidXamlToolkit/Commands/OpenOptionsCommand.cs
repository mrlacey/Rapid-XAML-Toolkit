// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class OpenOptionsCommand : BaseCommand
    {
        public const int CommandId = 4131;

        private OpenOptionsCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static OpenOptionsCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in OpenOptionsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new OpenOptionsCommand(package, commandService, logger);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (sender is OleMenuCommand menuCmd)
                {
                    menuCmd.Visible = menuCmd.Enabled = false;

                    if (!CodeParserBase.GetSettings().Profiles.Any())
                    {
                        menuCmd.Visible = menuCmd.Enabled = true;
                    }
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.Logger?.RecordFeatureUsage(nameof(OpenOptionsCommand));

                Type optionsPageType = typeof(SettingsConfigPage);
                this.AsyncPackage.ShowOptionPage(optionsPageType);
            }
            catch (Exception exc)
            {
                this.Logger?.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }
    }
}
