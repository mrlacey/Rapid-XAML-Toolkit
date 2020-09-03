// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class RowSpanOverflowTag : MissingDefinitionTag
    {
        public RowSpanOverflowTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT103", TagErrorType.Warning)
        {
            this.ToolTip = StringRes.UI_XamlAnalysisRowSpanOverflowTooltip;
        }
    }
}
