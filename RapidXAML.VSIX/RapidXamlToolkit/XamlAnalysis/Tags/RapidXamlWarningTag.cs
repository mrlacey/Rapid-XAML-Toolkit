// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlWarningTag : IRapidXamlWarningTag
    {
        public ActionTypes ActionType { get; protected set; }

        public Span Span { get; set; }

        public string ToolTip { get; set; }

        // This is shown in the description column
        public string Message { get; set; }

        // This is shown when the row is expanded
        public string ExtendedMessage { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public string ErrorCode { get; set; }

        public bool IsFatal { get; protected set; }

        public ITagSpan<IErrorTag> AsErrorTag()
        {
            var span = new SnapshotSpan(this.Snapshot, this.Span);
            return new TagSpan<IErrorTag>(span, new RapidXamlWarningAdornmentTag(this.ToolTip));
        }

        public ErrorRow AsErrorRow()
        {
            return new ErrorRow
            {
                ExtendedMessage = this.ExtendedMessage,
                Span = new SnapshotSpan(this.Snapshot, this.Span),
                Message = this.Message,
                ErrorCode = this.ErrorCode,
                IsFatal = this.IsFatal,
            };
        }
    }
}
