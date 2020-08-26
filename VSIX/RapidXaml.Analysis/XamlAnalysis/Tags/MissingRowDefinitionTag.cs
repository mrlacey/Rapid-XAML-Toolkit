// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingRowDefinitionTag : MissingDefinitionTag
    {
        public MissingRowDefinitionTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT101", TagErrorType.Warning)
        {
            this.ToolTip = StringRes.UI_XamlAnalysisMissingRowDefinitionTooltip;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisMissingRowDefinitionExtendedMessage;
        }
    }
}
