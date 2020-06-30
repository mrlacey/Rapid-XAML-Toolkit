// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace RapidXaml.AnalysisExe
{
    public class BuildAnalysisTextSnapshot : ITextSnapshot
    {
        private string rawText;

        public BuildAnalysisTextSnapshot(string fileName)
        {
            this.FileName = fileName;
        }

        public ITextBuffer TextBuffer { get; }

        public IContentType ContentType { get; }

        public ITextVersion Version { get; }

        public int Length => this.rawText?.Length ?? 0;

        public int LineCount { get; }

        public IEnumerable<ITextSnapshotLine> Lines { get; }

        public string FileName { get; }

        public char this[int position] => throw new NotImplementedException();

        public string GetText(Span span)
        {
            return string.Empty;
        }

        public string GetText(int startIndex, int length)
        {
            return string.Empty;
        }

        public string GetText()
        {
            if (string.IsNullOrWhiteSpace(this.rawText))
            {
                this.rawText = File.ReadAllText(this.FileName);
            }

            return this.rawText;
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
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
            return new BuildAnalysisTextSnapshotLine(this, position);
        }

        public int GetLineNumberFromPosition(int position)
        {
            throw new NotImplementedException();
        }

        public void Write(TextWriter writer, Span span)
        {
        }

        public void Write(TextWriter writer)
        {
        }
    }
}
