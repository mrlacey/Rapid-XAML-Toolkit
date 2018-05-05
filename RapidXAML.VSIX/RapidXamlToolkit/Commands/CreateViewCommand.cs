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
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    internal sealed class CreateViewCommand : BaseCommand
    {
        public const int CommandId = 4130;

        private CreateViewCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
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
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CreateViewCommand(package, commandService, logger);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (sender is OleMenuCommand menuCmd)
                {
                    menuCmd.Visible = menuCmd.Enabled = false;

                    uint itemid = VSConstants.VSITEMID_NIL;

                    if (!this.IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out itemid))
                    {
                        this.SelectedFileName = null;
                        return;
                    }

                    ((IVsProject)hierarchy).GetMkDocument(itemid, out string itemFullPath);
                    var transformFileInfo = new FileInfo(itemFullPath);

                    // Save the name of the selected file so we whave it when the command is executed
                    this.SelectedFileName = transformFileInfo.FullName;

                    if (transformFileInfo.Name.EndsWith(".cs") || transformFileInfo.Name.EndsWith(".vb"))
                    {
                        if (AnalyzerBase.GetSettings().IsActiveProfileSet)
                        {
                            menuCmd.Visible = menuCmd.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
                throw;
            }
        }

        private bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;
            int hr = VSConstants.S_OK;

            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (!(Package.GetGlobalService(typeof(SVsShellMonitorSelection)) is IVsMonitorSelection monitorSelection) || solution == null)
            {
                return false;
            }

            IVsMultiItemSelect multiItemSelect = null;
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null)
                {
                    return false;
                }

                // there is a hierarchy root node selected, thus it is not a single item inside a project
                if (itemid == VSConstants.VSITEMID_ROOT)
                {
                    return false;
                }

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null)
                {
                    return false;
                }

                Guid guidProjectID = Guid.Empty;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID)))
                {
                    return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
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

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.Logger?.RecordFeatureUsage(nameof(CreateViewCommand));

                this.Logger.RecordInfo("Attempting to create View.");
                var dte = this.ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE;
                var componentModel = (IComponentModel)this.ServiceProvider.GetServiceAsync(typeof(SComponentModel)).Result;

                var profile = AnalyzerBase.GetSettings().GetActiveProfile();

                var logic = new CreateViewCommandLogic(profile, this.Logger, new VisualStudioAbstraction(dte, componentModel));

                logic.Execute(this.SelectedFileName);

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
                    dte.ItemOperations.OpenFile(logic.XamlFileName, EnvDTE.Constants.vsViewKindDesigner);
                    this.Logger.RecordInfo($"Created file {logic.XamlFileName}");
                }
                else
                {
                    this.Logger.RecordInfo("No view created.");
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
