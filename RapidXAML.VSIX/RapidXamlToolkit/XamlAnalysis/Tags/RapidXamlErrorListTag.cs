// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlErrorListTag : RapidXamlAdornmentTag, IRapidXamlErrorListTag
    {
        protected RapidXamlErrorListTag(Span span, ITextSnapshot snapshot, string errorCode)
            : base(span, snapshot)
        {
            this.ErrorCode = errorCode;
        }

        public string Message { get; set; }

        // This is shown when the row is expanded
        public string ExtendedMessage { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tag is for something that should show in the Messages tab of the error list.
        /// </summary>
        public bool IsMessage { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tag is for something that should show in the Errors tab of the error list.
        /// This should never need setting.
        /// </summary>
        public bool IsError { get; protected set; }

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
