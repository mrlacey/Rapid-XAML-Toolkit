// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioTextManipulation
    {
        void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusions);

        void InsertIntoActiveDocumentOnNextLine(string text, int pos);

        void StartSingleUndoOperation(string name);

        void EndSingleUndoOperation();
    }
}
