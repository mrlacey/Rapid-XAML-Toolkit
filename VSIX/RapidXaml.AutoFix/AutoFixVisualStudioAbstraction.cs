// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using RapidXamlToolkit;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXaml
{
    public class AutoFixVisualStudioAbstraction : IVisualStudioAbstraction
    {
        public IProjectWrapper ActiveProject { get; set; } = null;

        public IProjectWrapper NamedProject { get; set; } = null;

        public bool UserConfirmsResult { get; set; } = false;

        public string ActiveDocumentFileName { get; set; }

        public string ActiveDocumentText { get; set; }

        public bool DocumentIsCSharp { get; set; } = false;

        public int CursorPosition { get; set; } = -1;

        public int LineNumber { get; set; } = 1;  // Assume that everything is on one line for testing. Bypasses the way VS adds extra char for end of line

        public int XamlIndent { get; set; } = 4;

        public IProjectWrapper GetActiveProject()
        {
            return this.ActiveProject;
        }

        public IProjectWrapper GetProject(string projectName)
        {
            return this.NamedProject;
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

        public void ReplaceInActiveDocOnLine(string find, string replace, int lineNumber)
        {
            // NOOP
        }

        public void ReplaceInActiveDoc(string find, string replace, int startIndex, int endIndex)
        {
            // NOOP
        }

        public void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusion)
        {
            // NOOP
        }

        public void InsertIntoActiveDocumentOnNextLine(string text, int pos)
        {
            // NOOP
        }

        public void InsertAtEndOfLine(int lineNumber, string toInsert)
        {
            // NOOP
        }

        public void DeleteFromEndOfLine(int lineNumber, int charsToDelete)
        {
            // NOOP
        }

        public bool StartSingleUndoOperation(string name)
        {
            return true;
        }

        public void EndSingleUndoOperation()
        {
            // NOOP
        }

        public async Task<int> GetXamlIndentAsync()
        {
            await Task.CompletedTask;
            return this.XamlIndent;
        }

        public void ReplaceInActiveDocOnLineOrAbove(string find, string replace, int lineNumber)
        {
            // NOOP
        }

        public void RemoveInActiveDocOnLine(string find, int lineNumber)
        {
            // NOOP
        }

        public ProjectType GetProjectType(EnvDTE.Project project)
        {
            return ProjectType.Unknown;
        }

        public string GetPathOfProjectContainingFile(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public (string projectFileName, ProjectType propjectType) GetNameAndTypeOfProjectContainingFile(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public void InsertIntoActiveDocOnLineAfterClosingTag(int openingAngleBracketLineNumber, string toInsert)
        {
            // NOOP
        }
    }
}
