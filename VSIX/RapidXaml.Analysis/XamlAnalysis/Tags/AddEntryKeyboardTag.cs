// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddEntryKeyboardTag : RapidXamlDisplayedTag
    {
        public AddEntryKeyboardTag(TagDependencies tagDependencies, string originalXaml)
            : base(tagDependencies, "RXT300", TagErrorType.Suggestion)
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
