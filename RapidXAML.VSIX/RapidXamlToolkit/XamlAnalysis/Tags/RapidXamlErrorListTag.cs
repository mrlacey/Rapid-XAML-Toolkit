// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlErrorListTag : RapidXamlAdornmentTag, IRapidXamlErrorListTag
    {
        protected RapidXamlErrorListTag(Span span, ITextSnapshot snapshot, string errorCode, int line, int column)
            : base(span, snapshot)
        {
            this.ErrorCode = errorCode;
            this.Line = line;
            this.Column = column;
        }

        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the message shown when the error row is expanded.
        /// </summary>
        public string ExtendedMessage { get; set; }

        public int Line { get; }

        public int Column { get; }

        /// <summary>
        /// Gets the code shown in the error list. Also used as the file name in the help link.
        /// </summary>
        public string ErrorCode { get; }

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
                Message = this.Description,
                ErrorCode = this.ErrorCode,
                IsMessage = this.IsMessage,
                IsError = this.IsError,
            };
        }
    }
}
