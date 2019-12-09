// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class RapidXamlParsingEventArgs : EventArgs
    {
        public RapidXamlParsingEventArgs(RapidXamlDocument document, string file, ITextSnapshot snapshot, ParsedAction action)
        {
            this.Document = document;
            this.File = file;
            this.Snapshot = snapshot;
            this.Action = action;
        }

        public RapidXamlDocument Document { get; private set; }

        public string File { get; private set; }

        public ITextSnapshot Snapshot { get; private set; }

        public ParsedAction Action { get; private set; }
    }
}
