// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioAbstraction : IVisualStudioTextManipulation, IVisualStudioProjectFilePath
    {
        bool UserConfirms(string title, string message);

        IProjectWrapper GetActiveProject();

        IProjectWrapper GetProject(string projectName);

        string GetActiveDocumentFileName();

        string GetActiveDocumentText();

        ProjectType GetProjectType(EnvDTE.Project project);

        // TODO: move this to a separate interface that AutoFix (& Build/AnalysisExe) don't need to know about
        (string projectFileName, ProjectType propjectType) GetNameAndTypeOfProjectContainingFile(string fileName);

        bool ActiveDocumentIsCSharp();

        // This should be the selection start if not a single point
        int GetCursorPosition();

        (int, int) GetCursorPositionAndLineNumber();

        Task<int> GetXamlIndentAsync();
    }
}
