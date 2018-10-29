// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
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

        public int CursorPosition { get; set; } = -1;

        public int LineNumber { get; set; } = 1;  // Assume that everything is on one line for testing. Bypasses the way VS adds extra char for end of line

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

        public int GetCursorPosition()
        {
            return this.CursorPosition;
        }

        public (int, int) GetCursorPositionAndLineNumber()
        {
            return (this.CursorPosition, this.LineNumber);
        }

        public void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusion)
        {
            // NOOP
        }
    }
}
