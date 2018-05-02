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

namespace RapidXamlToolkit
{
    public class VisualStudioAbstraction : IVisualStudioAbstraction
    {
        private readonly DTE dte;
        private readonly IComponentModel componentModel;

        public VisualStudioAbstraction(DTE dte, IComponentModel componentModel)
        {
            this.dte = dte ?? throw new ArgumentNullException(nameof(dte));
            this.componentModel = componentModel ?? throw new ArgumentNullException(nameof(componentModel));
        }

        public EnvDTE.Project GetActiveProject()
        {
            return ((Array)this.dte.ActiveSolutionProjects).GetValue(0) as EnvDTE.Project;
        }

        public (SyntaxTree syntaxTree, SemanticModel semModel) GetDocumentModels(string fileName)
        {
            var visualStudioWorkspace = this.componentModel.GetService<VisualStudioWorkspace>();

            Microsoft.CodeAnalysis.Solution solution = visualStudioWorkspace.CurrentSolution;
            DocumentId documentId = solution.GetDocumentIdsWithFilePath(fileName).FirstOrDefault();
            var document = solution.GetDocument(documentId);

            var root = document.GetSyntaxRootAsync().Result;
            var syntaxTree = root.SyntaxTree;

            var semModel = document.GetSemanticModelAsync().Result;

            return (syntaxTree, semModel);
        }

        public EnvDTE.Project GetProject(string projectName)
        {
            foreach (var project in this.dte.Solution.GetAllProjects())
            {
                if (project.Name == projectName)
                {
                    return project;
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
    }
}
