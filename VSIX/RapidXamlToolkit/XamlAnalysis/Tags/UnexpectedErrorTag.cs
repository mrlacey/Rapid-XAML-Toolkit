// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UnexpectedErrorTag : RapidXamlDisplayedTag
    {
        public UnexpectedErrorTag(Span span, ITextSnapshot snapshot, string fileName)
            : base(span, snapshot, fileName, "RXT999", TagErrorType.Error)
        {
            this.ToolTip = string.Empty;
            this.IsInternalError = true;
        }
    }
}
