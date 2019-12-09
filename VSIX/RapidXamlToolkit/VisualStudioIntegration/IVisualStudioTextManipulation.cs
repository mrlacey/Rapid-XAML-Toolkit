// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioTextManipulation
    {
        void RemoveInActiveDocOnLine(string find, int lineNumber);

        void ReplaceInActiveDocOnLine(string find, string replace, int lineNumber);

        void ReplaceInActiveDocOnLineOrAbove(string find, string replace, int lineNumber);

        void ReplaceInActiveDoc(string find, string replace, int startIndex, int endIndex);

        void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusions);

        void InsertIntoActiveDocumentOnNextLine(string text, int pos);

        void InsertAtEndOfLine(int lineNumber, string toInsert);

        void DeleteFromEndOfLine(int lineNumber, int charsToDelete);

        bool StartSingleUndoOperation(string name);

        void EndSingleUndoOperation();
    }
}
