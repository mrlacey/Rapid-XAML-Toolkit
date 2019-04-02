// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class NameTitleCaseTag : RapidXamlSuggestionTag
    {
        public NameTitleCaseTag(Span span, ITextSnapshot snapshot, int line, int column, string value)
            : base(span, snapshot, "RXT452", line, column)
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
