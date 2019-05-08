// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingColumnDefinitionTag : MissingDefinitionTag
    {
        public MissingColumnDefinitionTag(Span span, ITextSnapshot snapshot, string fileName, int line, int column)
            : base(span, snapshot, fileName, "RXT102", line, column, TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(AddMissingColumnDefinitionsAction);
            this.ToolTip = StringRes.Info_XamlAnalysisMissingColumnDefinitionTooltip;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisMissingColumnDefinitionExtendedMessage;
        }
    }
}
