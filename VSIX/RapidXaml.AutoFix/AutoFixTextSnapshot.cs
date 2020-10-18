// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.Tests
{
    public class AutoFixTextSnapshot : ITextSnapshotAbstraction
    {
        public AutoFixTextSnapshot(int length)
        {
            this.Length = length;
        }

        public int Length { get; }

        public object TextBuffer { get; }

        public int VersionNumber { get; }

        public (int StartPosition, int LineNumber) GetLineDetailsFromPosition(int position)
        {
            // This should never be called
            return (0, -1);
        }

        public int GetLineNumberFromPosition(int position)
        {
            // This should never be called
            return -1;
        }

        public string GetText()
        {
            throw new NotImplementedException();
        }
    }
}
