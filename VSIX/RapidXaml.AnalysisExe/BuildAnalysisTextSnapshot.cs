// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXaml.AnalysisExe
{
    public class BuildAnalysisTextSnapshot : ITextSnapshot, ITextSnapshotAbstraction
    {
        private string rawText;

        public BuildAnalysisTextSnapshot(string fileName)
        {
            this.FileName = fileName;
        }

        public int Length => this.rawText?.Length ?? 0;

        public object TextBuffer { get; set; }

        public int VersionNumber => 0;

        public IContentType ContentType { get; }

        public ITextVersion Version { get; }

        public int LineCount { get; }

        public IEnumerable<ITextSnapshotLine> Lines { get; }

        public string FileName { get; }

        ITextBuffer ITextSnapshot.TextBuffer => throw new NotImplementedException();

        public char this[int position] => throw new NotImplementedException();

        public string GetText()
        {
            if (string.IsNullOrWhiteSpace(this.rawText))
            {
                this.rawText = File.ReadAllText(this.FileName);
            }

            return this.rawText;
        }

        public (int StartPosition, int LineNumber) GetLineDetailsFromPosition(int position)
        {
            var line = new BuildAnalysisTextSnapshotLine(this, position);

            return (line.Start.Position, line.LineNumber);
        }

        public string GetText(Span span)
        {
            return string.Empty;
        }

        public string GetText(int startIndex, int length)
        {
            return string.Empty;
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

        public string GetLineTextFromLineNumber(int lineNo)
        {
            throw new NotImplementedException();
        }
    }
}
