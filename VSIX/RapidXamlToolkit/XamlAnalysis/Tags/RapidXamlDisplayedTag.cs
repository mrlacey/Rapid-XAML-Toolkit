// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlDisplayedTag : RapidXamlAdornmentTag, IRapidXamlErrorListTag
    {
        protected RapidXamlDisplayedTag(Span span, ITextSnapshot snapshot, string fileName, string errorCode, int line, int column, TagErrorType defaultErrorType)
            : base(span, snapshot, fileName)
        {
            this.ErrorCode = errorCode;
            this.Line = line;
            this.Column = column;
            this.DefaultErrorType = defaultErrorType;
        }

        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the message shown when the error row is expanded.
        /// </summary>
        public string ExtendedMessage { get; set; }

        public int Line { get; }

        public int Column { get; }

        public TagErrorType DefaultErrorType { get; }

        /// <summary>
        /// Gets the code shown in the error list. Also used as the file name in the help link.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the tag is for something that should show in the Errors tab of the error list.
        /// This should never need setting.
        /// </summary>
        public bool IsInternalError { get; protected set; }

        public TagErrorType ConfiguredErrorType
        {
            get
            {
                if (this.TryGetConfiguredErrorType(this.ErrorCode, out TagErrorType configuredType))
                {
                    return configuredType;
                }
                else
                {
                    return this.DefaultErrorType;
                }
            }
        }

        public bool TryGetConfiguredErrorType(string errorCode, out TagErrorType tagErrorType)
        {
            // tODO: Issue#140 implement TryGetConfiguredErrorType
            // get settings file if it exists
            // if value in config file return that.

            // Set to default if no override in file
            tagErrorType = this.DefaultErrorType;

            return false;
        }

        public override ITagSpan<IErrorTag> AsErrorTag()
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
                Message = this.Description,
                ErrorCode = this.ErrorCode,
                IsInternalError = this.IsInternalError,
                ErrorType = this.ConfiguredErrorType,
            };
        }
    }
}
