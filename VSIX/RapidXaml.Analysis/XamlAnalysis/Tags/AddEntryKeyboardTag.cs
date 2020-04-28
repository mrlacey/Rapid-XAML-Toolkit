// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddEntryKeyboardTag : RapidXamlDisplayedTag
    {
        public AddEntryKeyboardTag(Span span, ITextSnapshot snapshot, string fileName, string originalXaml, ILogger logger, IVisualStudioAbstraction vsa, string projectPath)
            : base(span, snapshot, fileName, "RXT300", TagErrorType.Suggestion, logger, vsa, projectPath)
        {
            this.SuggestedAction = typeof(AddEntryKeyboardAction);
            this.Description = StringRes.UI_XamlAnalysisEntryWithoutKeyboardDescription;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisEntryWithoutKeyboardExtendedMessage;

            var xaml = originalXaml.ToLowerInvariant();
            if (xaml.Contains("email"))
            {
                this.NonDefaultKeyboardSuggestion = "Email";
            }
            else if (xaml.Contains("phone") || xaml.Contains("cell") || xaml.Contains("mobile"))
            {
                this.NonDefaultKeyboardSuggestion = "Telephone";
            }
            else if (xaml.Contains("url"))
            {
                this.NonDefaultKeyboardSuggestion = "Url";
            }
        }

        public int InsertPosition { get; set; }

        public string NonDefaultKeyboardSuggestion { get; internal set; }
    }
}
