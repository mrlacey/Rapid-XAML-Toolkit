// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.Tests
{
    public class FakeTextSnapshotLine : ITextSnapshotLine
    {
        private readonly ITextSnapshot snapshot;

        public FakeTextSnapshotLine(ITextSnapshot snapshot)
        {
            this.snapshot = snapshot;
        }

        public ITextSnapshot Snapshot { get; }

        public SnapshotSpan Extent { get; }

        public SnapshotSpan ExtentIncludingLineBreak { get; }

        // This is currently sufficient for getting test to pass, but not a long term solution
        public int LineNumber => -1;

        // This is currently sufficient for getting test to pass, but not a long term solution
        public SnapshotPoint Start => new SnapshotPoint(this.snapshot, 0);

        public int Length { get; }

        public int LengthIncludingLineBreak { get; }

        public SnapshotPoint End { get; }

        public SnapshotPoint EndIncludingLineBreak { get; }

        public int LineBreakLength { get; }

        public string GetText()
        {
            throw new NotImplementedException();
        }

        public string GetTextIncludingLineBreak()
        {
            throw new NotImplementedException();
        }

        public string GetLineBreakText()
        {
            throw new NotImplementedException();
        }
    }
}
