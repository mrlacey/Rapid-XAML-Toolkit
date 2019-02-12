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

        public Type SuggestedAction { get; set; }

        public Span Span { get; set; }

        // TODO: Need to ensure not null?
        public string ToolTip { get; set; }

        public string Message { get; set; }

        // This is shown when the row is expanded
        public string ExtendedMessage { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public string ErrorCode { get; set; }

        public bool IsMessage { get; protected set; }

        public bool IsError { get; protected set; }

        public abstract ITagSpan<IErrorTag> AsErrorTag();
    }
}
