// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UidTitleCaseTag : RapidXamlSuggestionTag
    {
        public UidTitleCaseTag(Span span, ITextSnapshot snapshot, int line, int column, string value)
            : base(span, snapshot, "RXT451", line, column)
        {
            this.SuggestedAction = null;
            this.ToolTip = StringRes.Info_XamlAnalysisUidTitleCaseToolTip;
            this.Description = StringRes.Info_XamlAnalysisUidTitleCaseDescription.WithParams(value);
        }
    }
}
