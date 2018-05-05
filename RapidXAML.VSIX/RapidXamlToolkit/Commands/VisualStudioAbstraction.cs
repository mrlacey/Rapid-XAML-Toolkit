// <copyright file="VisualStudioAbstraction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text.Editor;

namespace RapidXamlToolkit
{
    public class VisualStudioAbstraction : IVisualStudioAbstraction
    {
        private readonly DTE dte;
        private readonly IComponentModel componentModel;
        private readonly IWpfTextView textView;

        public VisualStudioAbstraction(DTE dte, IComponentModel componentModel = null, IWpfTextView textView = null)
        {
            this.dte = dte ?? throw new ArgumentNullException(nameof(dte));
            this.componentModel = componentModel;
            this.textView = textView;
        }

        public string GetActiveDocumentFileName()
        {
            return this.dte.ActiveDocument.Name;
        }

        public string GetActiveDocumentText()
        {
            var activeDoc = this.dte.ActiveDocument;
            var objectDoc = activeDoc.Object("TextDocument") as EnvDTE.TextDocument;
            var docText = objectDoc.StartPoint.CreateEditPoint().GetText(objectDoc.EndPoint);

            return docText;
        }

        public ProjectWrapper GetActiveProject()
        {
            return new ProjectWrapper(((Array)this.dte.ActiveSolutionProjects).GetValue(0) as EnvDTE.Project);
        }

        public (SyntaxTree syntaxTree, SemanticModel semModel) GetDocumentModels(string fileName)
        {
            var visualStudioWorkspace = this.componentModel?.GetService<VisualStudioWorkspace>();

            Microsoft.CodeAnalysis.Solution solution = visualStudioWorkspace.CurrentSolution;
            DocumentId documentId = solution.GetDocumentIdsWithFilePath(fileName).FirstOrDefault();
            var document = solution.GetDocument(documentId);

            return this.GetDocumentModels(document);
        }

        public (SyntaxTree syntaxTree, SemanticModel semModel) GetDocumentModels(Microsoft.CodeAnalysis.Document document)
        {
            var root = document.GetSyntaxRootAsync().Result;
            var syntaxTree = root.SyntaxTree;

            var semModel = document.GetSemanticModelAsync().Result;

            return (syntaxTree, semModel);
        }

        public IWpfTextView GetActiveTextView()
        {
            return this.textView;
        }

        public ProjectWrapper GetProject(string projectName)
        {
            foreach (var project in this.dte.Solution.GetAllProjects())
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
            return this.dte.ActiveDocument.Language == "CSharp";
        }
    }
}
