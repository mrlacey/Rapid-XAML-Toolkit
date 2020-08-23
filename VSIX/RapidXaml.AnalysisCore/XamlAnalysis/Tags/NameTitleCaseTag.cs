// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class NameTitleCaseTag : RapidXamlDisplayedTag
    {
        public NameTitleCaseTag(TagDependencies tagDeps, string value)
            : base(tagDeps, "RXT452", TagErrorType.Suggestion)
        {
            this.SuggestedAction = typeof(MakeNameStartWithCapitalAction);
            this.ToolTip = StringRes.UI_XamlAnalysisNameTitleCaseToolTip;
            this.Description = StringRes.UI_XamlAnalysisNameTitleCaseDescription.WithParams(value);

            this.CurrentValue = value;
        }

        public string CurrentValue { get; private set; }

        public string DesiredValue => this.CurrentValue.Substring(0, 1).ToUpper() + this.CurrentValue.Substring(1);
    }
}
