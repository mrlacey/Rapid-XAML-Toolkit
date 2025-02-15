// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioAbstraction : IVisualStudioTextManipulation, IVisualStudioProjectFilePath
    {
        bool UserConfirms(string title, string message);

        string GetActiveDocumentText();

       // ProjectType GetProjectType(EnvDTE.Project project);

        (string ProjectFileName, ProjectType ProjectType) GetNameAndTypeOfProjectContainingFile(string fileName);

        (int Position, int LineNo) GetCursorPositionAndLineNumber();

        Task<int> GetXamlIndentAsync();

        string GetLanguageFromContainingProject(string fileName);

        List<string> GetFilesFromContainingProject(string fileName, params string[] fileNameEndings);
    }
}
