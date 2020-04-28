// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingRowDefinitionTag : MissingDefinitionTag
    {
        public MissingRowDefinitionTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT101", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(AddMissingRowDefinitionsAction);
            this.ToolTip = StringRes.UI_XamlAnalysisMissingRowDefinitionTooltip;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisMissingRowDefinitionExtendedMessage;
        }
    }
}
