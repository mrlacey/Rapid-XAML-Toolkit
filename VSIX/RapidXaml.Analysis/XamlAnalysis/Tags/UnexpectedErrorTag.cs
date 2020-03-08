// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UnexpectedErrorTag : RapidXamlDisplayedTag
    {
        // TODO: need to report errors from custom parsers differently.
        public UnexpectedErrorTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, "RXT999", TagErrorType.Error, logger)
        {
            this.ToolTip = string.Empty;
            this.IsInternalError = true;
        }
    }
}
