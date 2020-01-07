// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingColumnDefinitionTag : MissingDefinitionTag
    {
        public MissingColumnDefinitionTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, "RXT102", TagErrorType.Warning, logger)
        {
            this.SuggestedAction = typeof(AddMissingColumnDefinitionsAction);
            this.ToolTip = StringRes.Info_XamlAnalysisMissingColumnDefinitionTooltip;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisMissingColumnDefinitionExtendedMessage;
        }
    }
}
