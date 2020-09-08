// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioAbstraction : IVisualStudioTextManipulation
    {
        bool UserConfirms(string title, string message);

        ProjectWrapper GetActiveProject();

        ProjectWrapper GetProject(string projectName);

        string GetActiveDocumentFileName();

        string GetActiveDocumentText();

        ProjectType GetProjectType(EnvDTE.Project project);

        (string projectFileName, ProjectType propjectType) GetNameAndTypeOfProjectContainingFile(string fileName);

        bool ActiveDocumentIsCSharp();

        // This should be the selection start if not a single point
        int GetCursorPosition();

        (int, int) GetCursorPositionAndLineNumber();

        Task<int> GetXamlIndentAsync();
    }
}
