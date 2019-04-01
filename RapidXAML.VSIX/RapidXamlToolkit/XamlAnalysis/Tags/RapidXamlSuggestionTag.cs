// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlSuggestionTag : RapidXamlErrorListTag
    {
        protected RapidXamlSuggestionTag(Span span, ITextSnapshot snapshot, string errorCode, int line, int column)
            : base(span, snapshot, errorCode, line, column)
        {
            this.IsMessage = true;
        }

        public override ITagSpan<IErrorTag> AsErrorTag()
        {
            var span = new SnapshotSpan(this.Snapshot, this.Span);
            return new TagSpan<IErrorTag>(span, new RapidXamlSuggestionAdornmentTag(this.ToolTip));
        }
        }
}
