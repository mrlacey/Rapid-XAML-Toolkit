// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.Commands
{
    public interface IVisualStudioAbstraction
    {
        bool UserConfirms(string title, string message);

        ProjectWrapper GetActiveProject();

        ProjectWrapper GetProject(string projectName);

        Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(string fileName);

        Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(Document document);

        string GetActiveDocumentFileName();

        string GetActiveDocumentText();

        bool ActiveDocumentIsCSharp();

        // This should be the selection start if not a single point
        int GetCursorPosition();

        (int, int) GetCursorPositionAndLineNumber();

        void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusions);

        Task<int> GetXamlIndentAsync();
    }
}
