// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public class VisualStudioAbstraction : VisualStudioTextManipulation, IVisualStudioAbstraction
    {
        private readonly ILogger logger;
        private readonly IAsyncServiceProvider serviceProvider;
        private IComponentModel componentModel;
        private NuGet.VisualStudio.IVsPackageInstallerServices nugetService;

        // Pass in the DTE even though could get it from the ServiceProvider because it's needed in constructors but usage is async
        public VisualStudioAbstraction(ILogger logger, IAsyncServiceProvider serviceProvider, DTE dte)
        : base(dte)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider;
        }

        public string GetActiveDocumentFileName()
        {
            return this.Dte.ActiveDocument.Name;
        }

        public string GetActiveDocumentFilePath()
        {
            return this.Dte.ActiveDocument.FullName;
        }

        public ProjectType GetProjectType(EnvDTE.Project project)
        {
            const string WpfGuid = "{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}";
            const string UwpGuid = "{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}";
            const string XamAndroidGuid = "{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}";
            const string XamIosGuid = "{6BC8ED88-2882-458C-8E55-DFD12B67127B}";

            bool ReferencesXamarin(EnvDTE.Project proj)
            {
                return ReferencesNuGetPackageWithNameContaining(proj, "xamarin");
            }

            bool ReferencesUno(EnvDTE.Project proj)
            {
                return ReferencesNuGetPackageWithNameContaining(proj, "uno.ui");
            }

            bool ReferencesNuGetPackageWithNameContaining(EnvDTE.Project proj, string name)
            {
                try
                {
                    var nugetService = this.GetNuGetService(proj);

                    if (nugetService != null)
                    {
                        foreach (var item in nugetService.GetInstalledPackages(proj))
                        {
                            if (item?.Id.ToLowerInvariant().Contains(name) ?? false)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    this.logger?.RecordException(exc);
                }

                return false;
            }

            ProjectType GetProjectTypeOfReferencingProject(EnvDTE.Project proj)
            {
                // Look at other projects in solution that reference this project and see what they are.
                foreach (var otherProj in proj.DTE.Solution.GetAllProjects())
                {
                    // Don't check self or any shard projects to avoid infinite loops and unnecessary work.
                    // Unique name is ususally a relative path but may not be (esp. in .NETCore projects)
                    if (otherProj.UniqueName != project.UniqueName
                     && otherProj.UniqueName.EndsWith(".shproj", StringComparison.OrdinalIgnoreCase))
                    {
                        var item = otherProj.ProjectItems?.GetEnumerator();

                        // Can't get ProjectItems if project is unloaded.
                        if (item != null)
                        {
                            while (item.MoveNext())
                            {
                                var itemName = (item.Current as ProjectItem).Name;

                                if (itemName == $"<{proj.Name}>")
                                {
                                    var otherProjectType = this.GetProjectType(otherProj);

                                    if (otherProjectType != ProjectType.Unknown)
                                    {
                                        return otherProjectType;
                                    }
                                }
                            }

                            // As an alternative to trying to get project dependency from SDK format project files.
                            if (!string.IsNullOrEmpty(otherProj.FileName))
                            {
                                var projFileContents = System.IO.File.ReadAllText(otherProj.FileName);

                                if (projFileContents.Contains(proj.Name))
                                {
                                    var otherProjectType = this.GetProjectType(otherProj);

                                    if (otherProjectType != ProjectType.Unknown)
                                    {
                                        return otherProjectType;
                                    }
                                }
                            }
                        }
                    }
                }

                return ProjectType.Unknown;
            }

            var guids = this.GetProjectTypeGuids(project);

            // This will be the case in CPS projects
            if (string.IsNullOrWhiteSpace(guids))
            {
                if (this.ProjectUsesWpf(project))
                {
                    return ProjectType.Wpf;
                }
            }

            var result = ProjectType.Unknown;

            // Check with `Contains` as there may be multiple GUIDs specified (e.g. for programming language too)
            if (guids.IndexOf(WpfGuid, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                result = ProjectType.Wpf;
            }
            else if (guids.IndexOf(UwpGuid, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                // May be a UWP head for a XF app
                result = ReferencesXamarin(project) ? ProjectType.XamarinForms : ProjectType.Uwp;
            }
            else if (guids.IndexOf(XamAndroidGuid, StringComparison.InvariantCultureIgnoreCase) >= 0
                  || guids.IndexOf(XamIosGuid, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                result = ReferencesUno(project) ? ProjectType.Uwp : ProjectType.XamarinForms;
            }
            else
            {
                // Shared Projects provide no Guids or references that can be used to determine project type
                if (System.IO.Path.GetExtension(project.UniqueName).Equals(".shproj", StringComparison.OrdinalIgnoreCase))
                {
                    result = GetProjectTypeOfReferencingProject(project);
                }
                else
                {
                    var refsXamarin = ReferencesXamarin(project);
                    var refsUno = ReferencesUno(project);

                    if (refsXamarin)
                    {
                        result = refsUno ? ProjectType.Uwp : ProjectType.XamarinForms;
                    }
                    else
                    {
                        result = refsUno ? ProjectType.Uwp : ProjectType.Unknown;
                    }

                    // This will be the case if looking at code in a class library or other project types.
                    if (result == ProjectType.Unknown)
                    {
                        result = GetProjectTypeOfReferencingProject(project);
                    }
                }
            }

            return result;
        }

        public bool ProjectUsesWpf(EnvDTE.Project project)
        {
            var rawContent = System.IO.File.ReadAllText(project.FullName);

            return rawContent.Contains("<UseWPF>true</UseWPF>");
        }

        public string GetProjectTypeGuids(EnvDTE.Project proj)
        {
            string projectTypeGuids = string.Empty;

            try
            {
                object service = this.GetService(proj.DTE, typeof(IVsSolution));
                var solution = (IVsSolution)service;

                int result = solution.GetProjectOfUniqueName(proj.UniqueName, out IVsHierarchy hierarchy);

                if (result == 0)
                {
                    var aggregatableProject = (IVsAggregatableProject)hierarchy;
                    result = aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);
                }
            }
            catch (Exception exc)
            {
                // Some (unknown/irrelevant) project types may cause above casts to fail
                System.Diagnostics.Debug.WriteLine(exc);
            }

            return projectTypeGuids;
        }

        public object GetService(object serviceProvider, Type type)
        {
            return this.GetService(serviceProvider, type.GUID);
        }

        public object GetService(object serviceProviderObject, Guid guid)
        {
            object service = null;

            Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)serviceProviderObject;
            Guid serviceGuid = guid;
            Guid interopGuid = serviceGuid;
            int hresult = serviceProvider.QueryService(ref serviceGuid, ref interopGuid, out IntPtr serviceIntPtr);

            if (hresult != 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
            else if (!serviceIntPtr.Equals(IntPtr.Zero))
            {
                service = Marshal.GetObjectForIUnknown(serviceIntPtr);
                Marshal.Release(serviceIntPtr);
            }

            return service;
        }

        public string GetActiveDocumentText()
        {
            var activeDoc = this.Dte.ActiveDocument;

            if (activeDoc.Object("TextDocument") is EnvDTE.TextDocument objectDoc)
            {
                var docText = objectDoc.StartPoint.CreateEditPoint().GetText(objectDoc.EndPoint);

                return docText;
            }

            return null;
        }

        public ProjectWrapper GetActiveProject()
        {
            return new ProjectWrapper(((Array)this.Dte.ActiveSolutionProjects).GetValue(0) as EnvDTE.Project);
        }

        public async Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(string fileName)
        {
            var componentModel = await this.serviceProvider.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            var visualStudioWorkspace = componentModel?.GetService<VisualStudioWorkspace>();

            if (visualStudioWorkspace != null)
            {
                var solution = visualStudioWorkspace.CurrentSolution;
                var documentId = solution.GetDocumentIdsWithFilePath(fileName).FirstOrDefault();
                var document = solution.GetDocument(documentId);

                return await this.GetDocumentModelsAsync(document);
            }

            return (null, null);
        }

        public async Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(Microsoft.CodeAnalysis.Document document)
        {
            var root = await document.GetSyntaxRootAsync();
            var syntaxTree = root.SyntaxTree;

            var semModel = await document.GetSemanticModelAsync();

            return (syntaxTree, semModel);
        }

        public ProjectWrapper GetProject(string projectName)
        {
            foreach (var project in this.Dte.Solution.GetAllProjects())
            {
                if (project.Name == projectName)
                {
                    return new ProjectWrapper(project);
                }
            }

            return null;
        }

        public bool UserConfirms(string title, string message)
        {
            var msgResult = MessageBox.Show(
                                            message,
                                            title,
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Warning);

            return msgResult == MessageBoxResult.Yes;
        }

        public bool ActiveDocumentIsCSharp()
        {
            return this.Dte.ActiveDocument.Language == "CSharp";
        }

        public int GetCursorPosition()
        {
            var (offset, _) = this.GetCursorPositionAndLineNumber();

            return offset;
        }

        public (int, int) GetCursorPositionAndLineNumber()
        {
            var offset = ((TextSelection)this.Dte.ActiveDocument.Selection).AnchorPoint.AbsoluteCharOffset;
            var lineNo = ((TextSelection)this.Dte.ActiveDocument.Selection).CurrentLine;

            return (offset, lineNo);
        }

        public async Task<int> GetXamlIndentAsync()
        {
            try
            {
                var xamlLanguageGuid = new Guid("CD53C9A1-6BC2-412B-BE36-CC715ED8DD41");
                var languagePreferences = new LANGPREFERENCES3[1];

                languagePreferences[0].guidLang = xamlLanguageGuid;

                if (!(await this.serviceProvider.GetServiceAsync(typeof(SVsTextManager)) is IVsTextManager4 textManager))
                {
                    SharedRapidXamlPackage.Logger?.RecordGeneralError(StringRes.Error_FailedToGetIVsTextManager4InVisualStudioAbstraction_GetXamlIndentAsync);
                }
                else
                {
                    textManager.GetUserPreferences4(pViewPrefs: null, pLangPrefs: languagePreferences, pColorPrefs: null);

                    return (int)languagePreferences[0].uIndentSize;
                }
            }
            catch (Exception exc)
            {
                this.logger.RecordException(exc);
            }

            var indent = new IndentSize();

            return indent.Default;
        }

        public EnvDTE.Project GetProjectContainingFile(string fileName)
        {
            return this.Dte.Solution.GetProjectContainingFile(fileName);
        }

        private NuGet.VisualStudio.IVsPackageInstallerServices GetNuGetService(EnvDTE.Project proj)
        {
            // Cache these to avoid the overhead of looking them up multiple times for the same solution.
            if (this.componentModel is null)
            {
                this.componentModel = this.GetService(proj.DTE, typeof(SComponentModel)) as IComponentModel;
            }

            if (this.nugetService is null)
            {
                this.nugetService = this.componentModel.GetService<NuGet.VisualStudio.IVsPackageInstallerServices>();
            }

            return this.nugetService;
        }
    }
}
