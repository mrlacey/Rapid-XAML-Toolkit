// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.Tests
{
    public class FakeTextSnapshot : ITextSnapshot, ITextSnapshotAbstraction
    {
        public FakeTextSnapshot()
        {
        }

        public FakeTextSnapshot(int length)
        {
            this.Length = length;
        }

        public ITextBuffer TextBuffer { get; }

        public IContentType ContentType { get; }

        public ITextVersion Version { get; }

        public int Length { get; }

        public int LineCount { get; }

        public IEnumerable<ITextSnapshotLine> Lines { get; }

        object ITextSnapshotAbstraction.TextBuffer { get; }

        public int VersionNumber { get; }

        public char this[int position] => throw new NotImplementedException();

        public string GetText(Span span)
        {
            throw new NotImplementedException();
        }

        public string GetText(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public string GetText()
        {
            throw new NotImplementedException();
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshotLine GetLineFromPosition(int position)
        {
            return new FakeTextSnapshotLine(this);
        }

        public int GetLineNumberFromPosition(int position)
        {
            // This is sufficient for current testing needs
            return -1;
        }

        public void Write(TextWriter writer, Span span)
        {
            throw new NotImplementedException();
        }

        public void Write(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public (int StartPosition, int LineNumber) GetLineDetailsFromPosition(int position)
        {
            // Just need to return something
            return (0, 0);
        }
    }
}
