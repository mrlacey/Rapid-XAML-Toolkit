// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UidTitleCaseTag : RapidXamlDisplayedTag
    {
        public UidTitleCaseTag(TagDependencies tagDeps, string value)
            : base(tagDeps, "RXT451", TagErrorType.Suggestion)
        {
            this.SuggestedAction = typeof(MakeUidStartWithCapitalAction);
            this.ToolTip = StringRes.UI_XamlAnalysisUidTitleCaseToolTip;
            this.Description = StringRes.UI_XamlAnalysisUidTitleCaseDescription.WithParams(value);

            this.CurrentValue = value;
        }

        public string CurrentValue { get; private set; }

        public string DesiredValue => this.CurrentValue.Substring(0, 1).ToUpper() + this.CurrentValue.Substring(1);
    }
}
