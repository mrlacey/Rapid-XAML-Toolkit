// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXaml.AnalysisExe
{
    public class BuildAnalysisTextSnapshotLine : ITextSnapshotLineAbstraction
    {
        private readonly int lineNumber = 0;
        private readonly int start = 0;

        public BuildAnalysisTextSnapshotLine(ITextSnapshotAbstraction snapshot, int position)
        {
            var fileSoFar = snapshot.GetText().Substring(0, position);

            var endOfPreviousLine = fileSoFar.LastIndexOfAny(new char[] { '\r', '\n' });

            if (endOfPreviousLine > -1)
            {
                this.lineNumber = fileSoFar.Substring(0, endOfPreviousLine).Count(c => c == '\r');
                this.start = endOfPreviousLine;
            }
        }

        public int LineNumber => this.lineNumber;

        public int StartPosition => this.start;

        public int Length { get; }

        public int LengthIncludingLineBreak { get; }

        public int LineBreakLength { get; }

        public string GetText()
        {
            return string.Empty;
        }

        public string GetTextIncludingLineBreak()
        {
            return string.Empty;
        }

        public string GetLineBreakText()
        {
            return string.Empty;
        }
    }
}
