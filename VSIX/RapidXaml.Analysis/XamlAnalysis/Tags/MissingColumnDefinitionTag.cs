// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingColumnDefinitionTag : MissingDefinitionTag
    {
        public MissingColumnDefinitionTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT102", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(AddMissingColumnDefinitionsAction);
            this.ToolTip = StringRes.UI_XamlAnalysisMissingColumnDefinitionTooltip;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisMissingColumnDefinitionExtendedMessage;
        }
    }
}
