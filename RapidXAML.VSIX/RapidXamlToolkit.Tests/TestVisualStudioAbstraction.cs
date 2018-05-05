// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.Tests
{
    public class TestVisualStudioAbstraction : IVisualStudioAbstraction
    {
        public ProjectWrapper ActiveProject { get; set; } = null;

        public ProjectWrapper NamedProject { get; set; } = null;

        public SyntaxTree SyntaxTree { get; set; } = null;

        public SemanticModel SemanticModel { get; set; } = null;

        public bool UserConfirmsResult { get; set; } = false;

        public string ActiveDocumentFileName { get; set; }

        public string ActiveDocumentText { get; set; }

        public bool DocumentIsCSharp { get; set; } = false;

        public ProjectWrapper GetActiveProject()
        {
            return this.ActiveProject;
        }

        public ProjectWrapper GetProject(string projectName)
        {
            return this.NamedProject;
        }

        public (SyntaxTree syntaxTree, SemanticModel semModel) GetDocumentModels(string fileName)
        {
            return (this.SyntaxTree, this.SemanticModel);
        }

        public bool UserConfirms(string title, string message)
        {
            return this.UserConfirmsResult;
        }

        public string GetActiveDocumentFileName()
        {
            return this.ActiveDocumentFileName;
        }

        public string GetActiveDocumentText()
        {
            return this.ActiveDocumentText;
        }

        public (SyntaxTree syntaxTree, SemanticModel semModel) GetDocumentModels(Document document)
        {
            return (this.SyntaxTree, this.SemanticModel);
        }

        public bool ActiveDocumentIsCSharp()
        {
            return this.DocumentIsCSharp;
        }
    }
}
