// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddTextBoxInputScopeTag : RapidXamlDisplayedTag
    {
        public AddTextBoxInputScopeTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT150", TagErrorType.Suggestion)
        {
            this.Description = StringRes.UI_XamlAnalysisTextBoxWithoutInputScopeDescription;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisTextBoxWithoutInputScopeExtendedMessage;
        }

        public int InsertPosition { get; set; }
    }
}
