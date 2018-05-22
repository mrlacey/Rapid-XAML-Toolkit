// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RapidXamlToolkit.Commands;

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

        public async Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(string fileName)
        {
            await Task.CompletedTask;
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

        public async Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(Document document)
        {
            await Task.CompletedTask;
            return (this.SyntaxTree, this.SemanticModel);
        }

        public bool ActiveDocumentIsCSharp()
        {
            return this.DocumentIsCSharp;
        }
    }
}
