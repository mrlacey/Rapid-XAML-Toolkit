// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public interface IRapidXamlTextSnapshot
    {
        object TextBuffer { get; }

        int VersionNumber { get; }

        string GetText();

        int GetLineNumberFromPosition(int position);

        object GetLineFromPosition(int position);
    }

    public class RealTextSnapshot : IRapidXamlTextSnapshot
    {
        private readonly ITextSnapshot textSnapshot;

        public RealTextSnapshot(ITextSnapshot textSnapshot)
        {
            this.textSnapshot = textSnapshot;
        }

        public object TextBuffer
        {
            get
            {
                return this.textSnapshot.TextBuffer;
            }
        }

        public int VersionNumber
        {
            get
            {
                return this.textSnapshot.Version.VersionNumber;
            }
        }

        public string GetText()
        {
            return this.textSnapshot.GetText();
        }

        public int GetLineNumberFromPosition(int position)
        {
            return this.textSnapshot.GetLineNumberFromPosition(position);
        }

        public object GetLineFromPosition(int position)
        {
            return this.textSnapshot.GetLineFromPosition(position);
        }
    }

    // TODO: look at getting implementations from BuildAnalysisTextSnapshot
    public class CustomTextSnapshot : IRapidXamlTextSnapshot
    {
        public object TextBuffer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int VersionNumber
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string GetText()
        {
            throw new NotImplementedException();
        }

        public int GetLineNumberFromPosition(int position)
        {
            throw new NotImplementedException();
        }

        public object GetLineFromPosition(int position)
        {
            throw new NotImplementedException();
        }
    }
}
