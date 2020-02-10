// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UidTitleCaseTag : RapidXamlDisplayedTag
    {
        public UidTitleCaseTag(Span span, ITextSnapshot snapshot, string fileName, string value, ILogger logger)
            : base(span, snapshot, fileName, "RXT451", TagErrorType.Suggestion, logger)
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
