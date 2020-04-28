// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class RowSpanOverflowTag : MissingDefinitionTag
    {
        public RowSpanOverflowTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT103", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(RowSpanOverflowAction);
            this.ToolTip = StringRes.UI_XamlAnalysisRowSpanOverflowTooltip;
        }
    }
}
