// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.VisualStudioIntegration;

namespace Benchmarking
{
    public class BenchmarkingTextSnapshot : ITextSnapshotAbstraction
    {
        public BenchmarkingTextSnapshot(int length)
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

        public string GetLineTextFromLineNumber(int lineNo)
        {
            // This should never be called as only exists in an Action and this won't be invoked during benchmarking
            throw new System.NotImplementedException();
        }

        public string GetText()
        {
            // This should never be called
            throw new System.NotImplementedException();
        }
    }
}
