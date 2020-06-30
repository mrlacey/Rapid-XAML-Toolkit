// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class ColumnSpanOverflowTag : MissingDefinitionTag
    {
        public ColumnSpanOverflowTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT104", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(ColumnSpanOverflowAction);
            this.ToolTip = StringRes.UI_XamlAnalysisColumnSpanOverflowTooltip;
        }
    }
}
