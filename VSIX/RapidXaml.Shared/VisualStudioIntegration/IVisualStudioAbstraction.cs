// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RapidXamlToolkit.Commands;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioAbstraction : IVisualStudioTextManipulation
    {
        bool UserConfirms(string title, string message);

        ProjectWrapper GetActiveProject();

        ProjectWrapper GetProject(string projectName);

        Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(string fileName);

        Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(Document document);

        string GetActiveDocumentFileName();

        string GetActiveDocumentText();

        ProjectType GetProjectType(EnvDTE.Project project);

        EnvDTE.Project GetProjectContainingFile(string fileName);

        bool ActiveDocumentIsCSharp();

        // This should be the selection start if not a single point
        int GetCursorPosition();

        (int, int) GetCursorPositionAndLineNumber();

        Task<int> GetXamlIndentAsync();
    }
}
