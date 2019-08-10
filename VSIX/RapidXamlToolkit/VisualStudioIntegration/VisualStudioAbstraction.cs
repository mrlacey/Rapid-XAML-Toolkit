// Copyright (c) Microsoft Corporation. All rights reserved.
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
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public class VisualStudioAbstraction : VisualStudioTextManipulation, IVisualStudioAbstraction
    {
        private readonly ILogger logger;
        private readonly IAsyncServiceProvider serviceProvider;

        // Pass in the DTE even though could get it from the ServiceProvider because it's needed in constructors but usage is async
        public VisualStudioAbstraction(ILogger logger, IAsyncServiceProvider serviceProvider, DTE dte)
        : base(dte)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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
                return ReferencesNuGetPackageWithNameContaining(proj, ".uno");
            }

            bool ReferencesNuGetPackageWithNameContaining(EnvDTE.Project proj, string name)
            {
                var componentModel = this.GetService(proj.DTE, typeof(SComponentModel)) as IComponentModel;
                var nugetService = componentModel.GetService<NuGet.VisualStudio.IVsPackageInstallerServices>();

                if (nugetService != null)
                {
                    foreach (var item in nugetService.GetInstalledPackages(proj))
                    {
                        if (item.Id.ToLowerInvariant().Contains(name))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            var guids = this.GetProjectTypeGuids(project);

            // Check with `Contains` as there may be multiple GUIDs specified (e.g. for programming language too)
            if (guids.Contains(WpfGuid))
            {
                return ProjectType.Wpf;
            }
            else if (guids.Contains(UwpGuid))
            {
                // May be a UWP head for a XF app
                return ReferencesXamarin(project) ? ProjectType.XamarinForms : ProjectType.Uwp;
            }
            else if (guids.Contains(XamAndroidGuid) || guids.Contains(XamIosGuid))
            {
                return ReferencesUno(project) ? ProjectType.Uwp : ProjectType.XamarinForms;
            }
            else
            {
                // Shared Projects provide no Guids or references that can be used to determine project type
                if (System.IO.Path.GetExtension(project.FileName).Equals(".shproj", StringComparison.OrdinalIgnoreCase))
                {
                    // Look at other projects in solution that reference this project and see what they are.
                    foreach (var otherProj in project.DTE.Solution.GetAllProjects())
                    {
                        // Don't check self or any other shard projects to avoid infinite loops and unnecessary work.
                        if (!System.IO.Path.GetExtension(otherProj.UniqueName).Equals(".shproj", StringComparison.OrdinalIgnoreCase))
                        {
                            var item = otherProj.ProjectItems.GetEnumerator();

                            while (item.MoveNext())
                            {
                                var itemName = (item.Current as ProjectItem).Name;

                                if (itemName == $"<{project.Name}>")
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

                    return ProjectType.Unknown;
                }
                else
                {
                    var refsXamarin = ReferencesXamarin(project);
                    var refsUno = ReferencesUno(project);

                    if (refsXamarin)
                    {
                        return refsUno ? ProjectType.Uwp : ProjectType.XamarinForms;
                    }
                    else
                    {
                        return refsUno ? ProjectType.Uwp : ProjectType.Unknown;
                    }
                }
            }
        }

        public string GetProjectTypeGuids(EnvDTE.Project proj)
        {
            string projectTypeGuids = string.Empty;
            object service = null;
            IVsSolution solution = null;
            IVsHierarchy hierarchy = null;
            IVsAggregatableProject aggregatableProject = null;
            service = this.GetService(proj.DTE, typeof(IVsSolution));
            solution = (IVsSolution)service;

            try
            {
                int result = 0;
                result = solution.GetProjectOfUniqueName(proj.UniqueName, out hierarchy);

                if (result == 0)
                {
                    aggregatableProject = (IVsAggregatableProject)hierarchy;
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
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider = null;
            int hr = 0;
            Guid serviceGuid;
            Guid interopGuid;

            serviceGuid = guid;
            interopGuid = serviceGuid;
            serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)serviceProviderObject;
            hr = serviceProvider.QueryService(ref serviceGuid, ref interopGuid, out IntPtr serviceIntPtr);

            if (hr != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
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
                    RapidXamlPackage.Logger?.RecordError("Failed to get IVsTextManager4 in VisualStudioAbstraction.GetXamlIndentAsync");
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
    }
}
