// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class MoveAllHardCodedStringsToResourceFileCommand : BaseCommand
    {
        public const int CommandId = 4134;

        private MoveAllHardCodedStringsToResourceFileCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(RapidXamlPackage.AnalysisCommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static MoveAllHardCodedStringsToResourceFileCommand Instance
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
            // Verify the current thread is the UI thread - the call to AddCommand in the constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            Package = package;
            var commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new MoveAllHardCodedStringsToResourceFileCommand(package, commandService, logger);
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

                this.Logger?.RecordFeatureUsage(nameof(MoveAllHardCodedStringsToResourceFileCommand));

                var dte = await Instance.AsyncPackage.GetServiceAsync<DTE, DTE>();
                var vs = new VisualStudioAbstraction(this.Logger, this.AsyncPackage, dte);

                var filePath = vs.GetActiveDocumentFilePath();

                // Ensure that the open document has been saved or modifications may go wrong
                var rdt = new RunningDocumentTable(Package);
                rdt.SaveFileIfDirty(filePath);

                // Do this as don't have direct snapshot access and want any changes to be processed before making changes
                RapidXamlDocumentCache.TryUpdate(filePath);

                var tags = RapidXamlDocumentCache.ErrorListTags(filePath)
                                                 .OfType<HardCodedStringTag>()
                                                 .OrderByDescending(t => t.Span.Start);

                var referenceUids = new Dictionary<Guid, string>();

                vs.StartSingleUndoOperation(StringRes.UI_UndoContextMoveStringsToResourceFile);

                try
                {
                    foreach (var tag in tags)
                    {
                        if (!tag.UidExists && referenceUids.ContainsKey(tag.ElementGuid))
                        {
                            tag.UidExists = true;
                            tag.UidValue = referenceUids[tag.ElementGuid];
                        }

                        // pass in vs so have the same reference for tracking undo actions
                        var action = new HardCodedStringAction(filePath, null, tag, vs);
                        action.Execute(new CancellationTokenSource().Token);

                        if (tag.UidExists == false && tag.ElementGuid != Guid.Empty && !referenceUids.ContainsKey(tag.ElementGuid))
                        {
                            referenceUids.Add(tag.ElementGuid, tag.UidValue);
                        }
                    }
                }
                finally
                {
                    vs.EndSingleUndoOperation();
                }

                // Update again to force reflecting the changes that were just made.
                RapidXamlDocumentCache.TryUpdate(filePath);
            }
            catch (Exception exc)
            {
                this.Logger?.RecordException(exc);
                throw;
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = previousCursor;
            }
        }
    }
}
