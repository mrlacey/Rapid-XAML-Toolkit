// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class ColumnSpanOverflowTag : MissingDefinitionTag
    {
        public ColumnSpanOverflowTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger, IVisualStudioAbstraction vsa, string projectPath)
            : base(span, snapshot, fileName, "RXT104", TagErrorType.Warning, logger, vsa, projectPath)
        {
            this.SuggestedAction = typeof(ColumnSpanOverflowAction);
            this.ToolTip = StringRes.UI_XamlAnalysisColumnSpanOverflowTooltip;
        }
    }
}
