// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlErrorListTag : RapidXamlAdornmentTag, IRapidXamlErrorListTag
    {
        protected RapidXamlErrorListTag(Span span, ITextSnapshot snapshot)
            : base(span, snapshot)
        {
        }

        public ErrorRow AsErrorRow()
        {
            return new ErrorRow
            {
                ExtendedMessage = this.ExtendedMessage,
                Span = new SnapshotSpan(this.Snapshot, this.Span),
                Message = this.Message,
                ErrorCode = this.ErrorCode,
                IsMessage = this.IsMessage,
                IsError = this.IsError,
            };
        }
    }
}
