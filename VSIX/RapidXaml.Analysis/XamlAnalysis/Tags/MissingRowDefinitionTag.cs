// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingRowDefinitionTag : MissingDefinitionTag
    {
        public MissingRowDefinitionTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, "RXT101", TagErrorType.Warning, logger)
        {
            this.SuggestedAction = typeof(AddMissingRowDefinitionsAction);
            this.ToolTip = StringRes.Info_XamlAnalysisMissingRowDefinitionTooltip;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisMissingRowDefinitionExtendedMessage;
        }
    }
}
