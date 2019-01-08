// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal sealed class CreateViewCommand : BaseCommand
    {
        public const int CommandId = 4130;

        private CreateViewCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static CreateViewCommand Instance
        {
            get;
            private set;
        }

        private string SelectedFileName { get; set; }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in CreateViewCommand's constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CreateViewCommand(package, commandService, logger);
        }

        private async void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (sender is OleMenuCommand menuCmd)
                {
                    menuCmd.Visible = menuCmd.Enabled = false;

                    if (!this.IsSingleProjectItemSelection(out var hierarchy, out var itemid))
                    {
                        this.SelectedFileName = null;
                        return;
                    }

                    ((IVsProject)hierarchy).GetMkDocument(itemid, out var itemFullPath);
                    var transformFileInfo = new FileInfo(itemFullPath);

                    // Save the name of the selected file so we have it when the command is executed
                    this.SelectedFileName = transformFileInfo.FullName;

                    if (transformFileInfo.Name.EndsWith(".cs") || transformFileInfo.Name.EndsWith(".vb"))
                    {
                        if (CodeParserBase.GetSettings().IsActiveProfileSet)
                        {
                            menuCmd.Visible = menuCmd.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
            }
        }

        private bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;

            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (!(Package.GetGlobalService(typeof(SVsShellMonitorSelection)) is IVsMonitorSelection monitorSelection) || solution == null)
            {
                return false;
            }

            var hierarchyPtr = IntPtr.Zero;
            var selectionContainerPtr = IntPtr.Zero;

            try
            {
                var hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out var multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                if (multiItemSelect != null)
                {
                    // multiple items are selected
                    return false;
                }

                if (itemid == VSConstants.VSITEMID_ROOT)
                {
                    // there is a hierarchy root node selected, thus it is not a single item inside a project
                    return false;
                }

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null)
                {
                    return false;
                }

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out var _)))
                {
                    // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                    return false;
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

        private async void Execute(object sender, EventArgs e)
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                this.Logger?.RecordFeatureUsage(nameof(CreateViewCommand));

                this.Logger?.RecordInfo(StringRes.Info_AttemptingToCreateView);
                var dte = await this.ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;

                var logic = new CreateViewCommandLogic(this.Logger, new VisualStudioAbstraction(this.Logger, this.ServiceProvider, dte));

                await logic.ExecuteAsync(this.SelectedFileName);

                if (logic.CreateView)
                {
                    if (!Directory.Exists(logic.ViewFolder))
                    {
                        Directory.CreateDirectory(logic.ViewFolder);
                    }

                    File.WriteAllText(logic.XamlFileName, logic.XamlFileContents, Encoding.UTF8);
                    File.WriteAllText(logic.CodeFileName, logic.CodeFileContents, Encoding.UTF8);

                    // add files to project (rely on VS to nest them)
                    logic.ViewProject.Project.ProjectItems.AddFromFile(logic.XamlFileName);
                    logic.ViewProject.Project.ProjectItems.AddFromFile(logic.CodeFileName);

                    // Open the newly created view
                    dte?.ItemOperations.OpenFile(logic.XamlFileName, EnvDTE.Constants.vsViewKindDesigner);
                    this.Logger?.RecordInfo(StringRes.Info_CreatedView.WithParams(logic.XamlFileName));
                }
                else
                {
                    this.Logger?.RecordInfo(StringRes.Info_NoViewCreated);
                }
            }
            catch (Exception exc)
            {
                this.Logger?.RecordException(exc);
            }
        }
    }
}
