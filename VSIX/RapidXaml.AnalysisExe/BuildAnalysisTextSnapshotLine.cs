// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.Text;

namespace RapidXaml.AnalysisExe
{
    public class BuildAnalysisTextSnapshotLine : ITextSnapshotLine
    {
        private readonly ITextSnapshot snapshot;
        private readonly int lineNumber = 0;
        private readonly int start = 0;

        public BuildAnalysisTextSnapshotLine(ITextSnapshot snapshot, int position)
        {
            this.snapshot = snapshot;

            var fileSoFar = snapshot.GetText().Substring(0, position);

            var endOfPreviousLine = fileSoFar.LastIndexOfAny(new char[] { '\r', '\n' });

            if (endOfPreviousLine > -1)
            {
                this.lineNumber = fileSoFar.Substring(0, endOfPreviousLine).Count(c => c == '\r');
                this.start = endOfPreviousLine;
            }
        }

        public ITextSnapshot Snapshot { get; }

        public SnapshotSpan Extent { get; }

        public SnapshotSpan ExtentIncludingLineBreak { get; }

        public int LineNumber => this.lineNumber;

        public SnapshotPoint Start => new SnapshotPoint(this.snapshot, this.start);

        public int Length { get; }

        public int LengthIncludingLineBreak { get; }

        public SnapshotPoint End { get; }

        public SnapshotPoint EndIncludingLineBreak { get; }

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
