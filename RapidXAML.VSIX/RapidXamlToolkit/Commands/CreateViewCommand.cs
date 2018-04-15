// <copyright file="CreateViewCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    internal sealed class CreateViewCommand
    {
        public const int CommandId = 4130;

        public static readonly Guid CommandSet = new Guid("8c20aab1-50b0-4523-8d9d-24d512fa8154");

        private readonly AsyncPackage package;

        private CreateViewCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
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

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        private string SelectedFileName { get; set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in CreateViewCommand's constructor requires the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CreateViewCommand(package, commandService);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
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

                this.SelectedFileName = transformFileInfo.FullName;

                if (transformFileInfo.Name.EndsWith(".cs") || transformFileInfo.Name.EndsWith(".vb"))
                {
                    menuCmd.Visible = menuCmd.Enabled = true;
                }
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
            // get current project
            var dte = this.ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE;

            // This may need to be configurable to support multi-project files (ISSUE#21)
            var proj = ((Array)dte.ActiveSolutionProjects).GetValue(0) as EnvDTE.Project;

            var fileExt = Path.GetExtension(this.SelectedFileName);
            var fileContents = File.ReadAllText(this.SelectedFileName);

            var profile = AnalyzerBase.GetSettings().GetActiveProfile();

            AnalyzerBase analyzer = null;
            SyntaxTree syntaxTree = null;
            SemanticModel semModel = null;
            string codeBehindExt = string.Empty;

            switch (fileExt)
            {
                case ".cs":
                    analyzer = new CSharpAnalyzer();
                    syntaxTree = CSharpSyntaxTree.ParseText(fileContents);
                    semModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, ignoreAccessibility: true);
                    codeBehindExt = (analyzer as CSharpAnalyzer).FileExtension;
                    break;
                case ".vb":
                    analyzer = new VisualBasicAnalyzer();
                    syntaxTree = VisualBasicSyntaxTree.ParseText(fileContents);
                    semModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, ignoreAccessibility: true);
                    codeBehindExt = (analyzer as VisualBasicAnalyzer).FileExtension;
                    break;
            }

            if (analyzer != null)
            {
                // Index Of is allowing for "class " in C# and "Class " in VB
                var actual = (analyzer as IDocumentAnalyzer).GetSingleItemOutput(syntaxTree.GetRoot(), semModel, fileContents.IndexOf("lass "), profile);

                var className = actual.Name;

                // Should this be configurable? Should the file name match the source file name or the name of the class in the file? (ISSUE#21)
                var baseFileName = Path.GetFileNameWithoutExtension(this.SelectedFileName);
                var folder = Path.GetDirectoryName(proj.FileName);

                // This should be configurable (ISSUE#21)
                var viewFolder = Path.Combine(folder, "Views");

                if (!Directory.Exists(viewFolder))
                {
                    Directory.CreateDirectory(viewFolder);
                }

                // This should be configurable (ISSUE#21)
                var xamlFileName = Path.Combine(viewFolder, $"{baseFileName}Page.xaml");
                var codeFileName = Path.Combine(viewFolder, $"{baseFileName}Page.xaml.{codeBehindExt}");

                // TODO: This should be the name of the project that the file will be added to. (this may be differnt to the one the VM is in)
                var projName = proj.Name;

                var xamlContent = profile.ViewGeneration.XamlPlaceholder.Replace("$project$", projName).Replace("$class$", className).Replace("$genxaml$", actual.Output);

                // The content should be configurable (ISSUE#21)
                File.WriteAllText(xamlFileName, xamlContent, Encoding.UTF8);

                var codeBehind = profile.ViewGeneration.CodePlaceholder.Replace("$project$", projName).Replace("$class$", className);

                // The content should be configurable (ISSUE#21)
                File.WriteAllText(codeFileName, codeBehind, Encoding.UTF8);

                // add files to project (rely on VS to nest them)
                proj.ProjectItems.AddFromFile(xamlFileName);
                proj.ProjectItems.AddFromFile(codeFileName);

                // Open the newly created view
                dte.ItemOperations.OpenFile(xamlFileName, EnvDTE.Constants.vsViewKindDesigner);
            }
        }
    }
}
