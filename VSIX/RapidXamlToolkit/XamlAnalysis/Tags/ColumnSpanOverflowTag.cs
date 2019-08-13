// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class ColumnSpanOverflowTag : MissingDefinitionTag
    {
        public ColumnSpanOverflowTag(Span span, ITextSnapshot snapshot, string fileName)
            : base(span, snapshot, fileName, "RXT104", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(ColumnSpanOverflowAction);
            this.ToolTip = StringRes.Info_XamlAnalysisColumnSpanOverflowTooltip;
        }
    }
}
