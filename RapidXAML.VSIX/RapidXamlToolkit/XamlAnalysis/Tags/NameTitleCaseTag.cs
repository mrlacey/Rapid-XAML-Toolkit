// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class NameTitleCaseTag : RapidXamlSuggestionTag
    {
        public NameTitleCaseTag(Span span, ITextSnapshot snapshot, int line, int column, string value)
            : base(span, snapshot, "RXT452", line, column)
        {
            this.SuggestedAction = null;
            this.ToolTip = StringRes.Info_XamlAnalysisNameTitleCaseToolTip;
            this.Description = StringRes.Info_XamlAnalysisNameTitleCaseDescription.WithParams(value);
        }
    }
}
