// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Logging;

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

            var indent = new Microsoft.VisualStudio.Text.Editor.IndentSize();

            return indent.Default;
        }
    }
}
