// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingRowDefinitionTag : MissingDefinitionTag
    {
        public MissingRowDefinitionTag(Span span, ITextSnapshot snapshot, int line, int column)
            : base(span, snapshot, "RXT101", line, column, TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(AddMissingRowDefinitionsAction);
            this.ToolTip = StringRes.Info_XamlAnalysisMissingRowDefinitionTooltip;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisMissingRowDefinitionExtendedMessage;
        }
    }
}
