// <copyright file="CreateViewCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
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

            AnalyzerBase.ServiceProvider = (IServiceProvider)Instance.ServiceProvider;
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
                this.Logger.RecordInfo("Attempting to create View.");
                var dte = this.ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE;

                var vmProj = ((Array)dte.ActiveSolutionProjects).GetValue(0) as EnvDTE.Project;

                var fileExt = Path.GetExtension(this.SelectedFileName);
                var fileContents = File.ReadAllText(this.SelectedFileName);

                var profile = AnalyzerBase.GetSettings().GetActiveProfile();

                var componentModel = (IComponentModel)this.ServiceProvider.GetServiceAsync(typeof(SComponentModel)).Result;
                var visualStudioWorkspace = componentModel.GetService<VisualStudioWorkspace>();

                Microsoft.CodeAnalysis.Solution solution = visualStudioWorkspace.CurrentSolution;
                DocumentId documentId = solution.GetDocumentIdsWithFilePath(this.SelectedFileName).FirstOrDefault();
                var document = solution.GetDocument(documentId);

                var root = document.GetSyntaxRootAsync().Result;
                var syntaxTree = root.SyntaxTree;

                var semModel = document.GetSemanticModelAsync().Result;

                AnalyzerBase analyzer = null;
                string codeBehindExt = string.Empty;

                switch (fileExt)
                {
                    case ".cs":
                        analyzer = new CSharpAnalyzer(this.Logger);
                        codeBehindExt = (analyzer as CSharpAnalyzer).FileExtension;
                        break;
                    case ".vb":
                        analyzer = new VisualBasicAnalyzer(this.Logger);
                        codeBehindExt = (analyzer as VisualBasicAnalyzer).FileExtension;
                        break;
                }

                if (analyzer != null)
                {
                    // Index Of is allowing for "class " in C# and "Class " in VB
                    var analyzerOutput = (analyzer as IDocumentAnalyzer).GetSingleItemOutput(syntaxTree.GetRoot(), semModel, fileContents.IndexOf("lass "), profile);

                    var config = profile.ViewGeneration;

                    var vmClassName = analyzerOutput.Name;

                    var baseClassName = vmClassName;

                    if (vmClassName.EndsWith(config.ViewModelFileSuffix))
                    {
                        baseClassName = vmClassName.Substring(0, vmClassName.LastIndexOf(config.ViewModelFileSuffix));
                    }

                    var viewClassName = $"{baseClassName}{config.XamlFileSuffix}";

                    var vmProjName = vmProj.Name;
                    var viewProjName = string.Empty;

                    EnvDTE.Project viewProj = null;

                    if (config.AllInSameProject)
                    {
                        viewProj = vmProj;
                        viewProjName = viewProj.Name;
                    }
                    else
                    {
                        var expectedViewProjectName = vmProjName.Replace(config.ViewModelProjectSuffix, config.XamlProjectSuffix);

                        foreach (var project in dte.Solution.GetAllProjects())
                        {
                            if (project.Name == expectedViewProjectName)
                            {
                                viewProj = project;
                                break;
                            }
                        }

                        if (viewProj == null)
                        {
                            this.Logger.RecordError($"Unable to find project '{expectedViewProjectName}' in the solution.");
                        }

                        viewProjName = viewProj?.Name;
                    }

                    if (viewProj != null)
                    {
                        var folder = Path.GetDirectoryName(viewProj.FileName);

                        var viewFolder = Path.Combine(folder, config.XamlFileDirectoryName);

                        // We assume that the type name matches the file name.
                        var xamlFileName = Path.Combine(viewFolder, $"{viewClassName}.xaml");
                        var codeFileName = Path.Combine(viewFolder, $"{viewClassName}.xaml.{codeBehindExt}");

                        var createFile = true;

                        if (File.Exists(xamlFileName))
                        {
                            this.Logger.RecordInfo($"File '{xamlFileName}' already exists");

                            var msgResult = MessageBox.Show(
                                                       $"Do you want to override the existing file?",
                                                       "File already exists",
                                                       MessageBoxButton.YesNo,
                                                       MessageBoxImage.Warning);

                            if (msgResult != MessageBoxResult.Yes)
                            {
                                createFile = false;
                                this.Logger.RecordInfo($"Not overwriting '{xamlFileName}'");
                            }
                            else
                            {
                                this.Logger.RecordInfo($"Overwriting '{xamlFileName}'");
                            }
                        }

                        if (createFile)
                        {
                            var viewNamespace = $"{viewProjName}.{config.XamlFileDirectoryName}".TrimEnd('.');
                            var vmNamespace = $"{vmProjName}.{config.ViewModelDirectoryName}".TrimEnd('.');

                            if (!Directory.Exists(viewFolder))
                            {
                                Directory.CreateDirectory(viewFolder);
                            }

                            var replacementValues = (viewProjName, viewNamespace, vmNamespace, viewClassName, vmClassName, analyzerOutput.Output);

                            var xamlContent = this.ReplacePlaceholders(config.XamlPlaceholder, replacementValues);
                            File.WriteAllText(xamlFileName, xamlContent, Encoding.UTF8);

                            var codeBehind = this.ReplacePlaceholders(config.CodePlaceholder, replacementValues);
                            File.WriteAllText(codeFileName, codeBehind, Encoding.UTF8);

                            // add files to project (rely on VS to nest them)
                            viewProj.ProjectItems.AddFromFile(xamlFileName);
                            viewProj.ProjectItems.AddFromFile(codeFileName);

                            // Open the newly created view
                            dte.ItemOperations.OpenFile(xamlFileName, EnvDTE.Constants.vsViewKindDesigner);
                            this.Logger.RecordInfo($"Created file {xamlFileName}");
                        }
                    }
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

        private string ReplacePlaceholders(string source, (string projName, string viewNs, string vmNs, string viewClass, string vmClass, string xaml) values)
        {
            return source.Replace(Placeholder.ViewProject, values.projName)
                         .Replace(Placeholder.ViewNamespace, values.viewNs)
                         .Replace(Placeholder.ViewModelNamespace, values.vmNs)
                         .Replace(Placeholder.ViewClass, values.viewClass)
                         .Replace(Placeholder.ViewModelClass, values.vmClass)
                         .Replace(Placeholder.GeneratedXAML, values.xaml);
        }
    }
}
