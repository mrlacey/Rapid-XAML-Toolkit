// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface ITextSnapshotAbstraction
    {
        int Length { get; }

        object TextBuffer { get; }

        int VersionNumber { get; }

        string GetText();

        (int StartPosition, int LineNumber) GetLineDetailsFromPosition(int position);

        int GetLineNumberFromPosition(int position);
    }
}
