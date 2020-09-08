// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioAbstraction : IVisualStudioTextManipulation, IVisualStudioProjectFilePath
    {
        bool UserConfirms(string title, string message);

        string GetActiveDocumentText();

        ProjectType GetProjectType(EnvDTE.Project project);

        // TO DO: move this to a separate interface that AutoFix (& Build/AnalysisExe) don't need to know about
        (string projectFileName, ProjectType propjectType) GetNameAndTypeOfProjectContainingFile(string fileName);

        (int, int) GetCursorPositionAndLineNumber();

        Task<int> GetXamlIndentAsync();
    }
}
