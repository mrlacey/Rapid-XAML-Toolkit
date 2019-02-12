// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlAdornmentTag : IRapidXamlAdornmentTag
    {
        private readonly ITextSnapshot _snapshot;

        public RapidXamlAdornmentTag(Span span, ITextSnapshot snapshot)
        {
            this.Span = span;
            this.Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }

        public string ToolTip { get; set; }

        public Type SuggestedAction { get; set; }

        public Span Span { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public abstract ITagSpan<IErrorTag> AsErrorTag();
    }
}
