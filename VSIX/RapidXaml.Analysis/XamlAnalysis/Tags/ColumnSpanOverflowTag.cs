// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class ColumnSpanOverflowTag : MissingDefinitionTag
    {
        public ColumnSpanOverflowTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT104", TagErrorType.Warning)
        {
            this.ToolTip = StringRes.UI_XamlAnalysisColumnSpanOverflowTooltip;
        }
    }
}
