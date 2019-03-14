// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UnexpectedErrorTag : RapidXamlWarningTag
    {
        public UnexpectedErrorTag(Span span, ITextSnapshot snapshot)
            : base(span, snapshot, "RXT999", 0, 0)
        {
            this.ToolTip = string.Empty;
            this.IsError = true;
        }
    }
}
