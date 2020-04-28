// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UnexpectedErrorTag : RapidXamlDisplayedTag
    {
        public UnexpectedErrorTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger, IVisualStudioAbstraction vsa)
            : base(span, snapshot, fileName, "RXT999", TagErrorType.Error, logger, vsa, string.Empty)
        {
            this.ToolTip = string.Empty;
            this.IsInternalError = true;
        }
    }
}
