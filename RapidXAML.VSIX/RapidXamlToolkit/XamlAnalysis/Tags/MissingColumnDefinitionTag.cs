// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingColumnDefinitionTag : MissingDefinitionTag
    {
        // TODO: Need to assign a proper error code
        public MissingColumnDefinitionTag(Span span, ITextSnapshot snapshot, int line, int column)
            : base(span, snapshot, "RXT???", line, column)
        {
            this.SuggestedAction = typeof(AddMissingColumnDefinitionsAction);
            this.ToolTip = StringRes.Info_XamlAnalysisMissingColumnDefinitionTooltip;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisMissingColumnDefinitionExtendedMessage;
        }
    }
}
