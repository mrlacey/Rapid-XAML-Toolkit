// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlAdornmentTag : IRapidXamlAdornmentTag
    {
        public RapidXamlAdornmentTag(Span span, ITextSnapshot snapshot, string fileName)
        {
            this.Span = span;
            this.Snapshot = snapshot;
            this.FileName = fileName;
        }

        public string ToolTip { get; set; }

        public Type SuggestedAction { get; set; }

        public Span Span { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public string FileName { get; }

        public abstract ITagSpan<IErrorTag> AsErrorTag();
    }
}
