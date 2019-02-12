// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlWarningTag : RapidXamlErrorListTag
    {
        protected RapidXamlWarningTag(Span span, ITextSnapshot snapshot, string errorCode)
            : base(span, snapshot, errorCode)
        {
        }

        public override ITagSpan<IErrorTag> AsErrorTag()
        {
            var span = new SnapshotSpan(this.Snapshot, this.Span);
            return new TagSpan<IErrorTag>(span, new RapidXamlWarningAdornmentTag(this.ToolTip));
        }
    }
}
