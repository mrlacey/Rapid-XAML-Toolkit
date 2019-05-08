// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class RowSpanOverflowTag : MissingDefinitionTag
    {
        public RowSpanOverflowTag(Span span, ITextSnapshot snapshot, string fileName, int line, int column)
            : base(span, snapshot, "RXT103", fileName, line, column, TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(RowSpanOverflowAction);
            this.ToolTip = StringRes.Info_XamlAnalysisRowSpanOverflowTooltip;
        }
    }
}
