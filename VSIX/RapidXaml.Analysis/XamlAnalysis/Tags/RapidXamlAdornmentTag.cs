// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlAdornmentTag : IRapidXamlAdornmentTag
    {
        public RapidXamlAdornmentTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger)
        {
            this.Span = span;
            this.Snapshot = snapshot;
            this.FileName = fileName;
            this.Logger = logger;
        }

        public string ToolTip { get; set; }

        public Type SuggestedAction { get; set; }

        public Span Span { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public string FileName { get; }

        public ILogger Logger { get; }

        public string ErrorCode { get; set; }

        public abstract ITagSpan<IErrorTag> AsErrorTag();
    }
}
