// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UnexpectedErrorTag : RapidXamlWarningTag
    {
        public UnexpectedErrorTag(Span span, ITextSnapshot snapshot)
            : base(span, snapshot)
        {
            this.ActionType = ActionTypes.UnexpectedError;
            this.ToolTip = string.Empty;
            this.ErrorCode = "RXT999";
            this.IsError = true;
        }
    }
}
