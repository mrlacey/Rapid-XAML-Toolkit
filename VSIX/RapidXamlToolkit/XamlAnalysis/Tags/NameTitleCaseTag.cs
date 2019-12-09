// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class NameTitleCaseTag : RapidXamlDisplayedTag
    {
        public NameTitleCaseTag(Span span, ITextSnapshot snapshot, string fileName, string value)
            : base(span, snapshot, fileName, "RXT452",  TagErrorType.Suggestion)
        {
            this.SuggestedAction = typeof(MakeNameStartWithCapitalAction);
            this.ToolTip = StringRes.Info_XamlAnalysisNameTitleCaseToolTip;
            this.Description = StringRes.Info_XamlAnalysisNameTitleCaseDescription.WithParams(value);

            this.CurrentValue = value;
        }

        public string CurrentValue { get; private set; }

        public string DesiredValue => this.CurrentValue.Substring(0, 1).ToUpper() + this.CurrentValue.Substring(1);
    }
}
