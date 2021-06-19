// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXaml.AnalysisExe
{
    public class BuildAnalysisTextSnapshot : ITextSnapshotAbstraction
    {
        private string rawText;

        public BuildAnalysisTextSnapshot(string fileName)
        {
            this.FileName = fileName;
        }

        public int Length => this.rawText?.Length ?? 0;

        public object TextBuffer { get; set; }

        public int VersionNumber => 0;

        public int LineCount { get; }

        public string FileName { get; }

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

            return (line.StartPosition, line.LineNumber);
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

        public ITextSnapshotLineAbstraction GetLineFromPosition(int position)
        {
            return new BuildAnalysisTextSnapshotLine(this, position);
        }

        public int GetLineNumberFromPosition(int position)
        {
            throw new NotImplementedException();
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
