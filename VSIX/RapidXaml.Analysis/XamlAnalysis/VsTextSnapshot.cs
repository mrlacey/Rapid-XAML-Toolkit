// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis
{
    internal class VsTextSnapshot : ITextSnapshotAbstraction
    {
        private readonly ITextSnapshot underlyingSnapshot;

        public VsTextSnapshot(ITextSnapshot snapshot)
        {
            this.underlyingSnapshot = snapshot;
        }

        public int Length => this.underlyingSnapshot.Length;

        public object TextBuffer => this.underlyingSnapshot.TextBuffer;

        public int VersionNumber => this.underlyingSnapshot.Version.VersionNumber;

        public (int StartPosition, int LineNumber) GetLineDetailsFromPosition(int position)
        {
            var line = this.underlyingSnapshot.GetLineFromPosition(position);

            return (line.Start.Position, line.LineNumber);
        }

        public int GetLineNumberFromPosition(int position)
        {
            return this.underlyingSnapshot.GetLineFromPosition(position).LineNumber;
        }

        public string GetLineTextFromLineNumber(int lineNo)
        {
            return this.underlyingSnapshot.GetLineFromLineNumber(lineNo).GetText();
        }

        public string GetText()
        {
            return this.underlyingSnapshot.GetText();
        }
    }
}
