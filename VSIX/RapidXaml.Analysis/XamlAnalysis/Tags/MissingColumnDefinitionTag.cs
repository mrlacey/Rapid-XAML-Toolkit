// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingColumnDefinitionTag : MissingDefinitionTag
    {
        public MissingColumnDefinitionTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT102", TagErrorType.Warning)
        {
            this.ToolTip = StringRes.UI_XamlAnalysisMissingColumnDefinitionTooltip;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisMissingColumnDefinitionExtendedMessage;
        }
    }
}
