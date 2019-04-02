// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UidTitleCaseTag : RapidXamlSuggestionTag
    {
        public UidTitleCaseTag(Span span, ITextSnapshot snapshot, int line, int column, string value)
            : base(span, snapshot, "RXT451", line, column)
        {
            this.SuggestedAction = typeof(MakeUidStartWithCapitalAction);
            this.ToolTip = StringRes.Info_XamlAnalysisUidTitleCaseToolTip;
            this.Description = StringRes.Info_XamlAnalysisUidTitleCaseDescription.WithParams(value);

            this.CurrentValue = value;
        }

        public string CurrentValue { get; private set; }

        public string DesiredValue => this.CurrentValue.Substring(0, 1).ToUpper() + this.CurrentValue.Substring(1);
    }
}
