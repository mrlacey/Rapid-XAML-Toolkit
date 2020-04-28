// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingRowDefinitionTag : MissingDefinitionTag
    {
        public MissingRowDefinitionTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger, IVisualStudioAbstraction vsa, string projectPath)
            : base(span, snapshot, fileName, "RXT101", TagErrorType.Warning, logger, vsa, projectPath)
        {
            this.SuggestedAction = typeof(AddMissingRowDefinitionsAction);
            this.ToolTip = StringRes.UI_XamlAnalysisMissingRowDefinitionTooltip;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisMissingRowDefinitionExtendedMessage;
        }
    }
}
