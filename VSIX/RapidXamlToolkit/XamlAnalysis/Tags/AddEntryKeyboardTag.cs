// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddEntryKeyboardTag : RapidXamlSuggestionTag
    {
        public AddEntryKeyboardTag(Span span, ITextSnapshot snapshot, int line, int column, string originalXaml)
            : base(span, snapshot, "RXT300", line, column)
        {
            this.SuggestedAction = typeof(AddEntryKeyboardAction);
            this.Description = StringRes.Info_XamlAnalysisEntryWithoutKeyboardDescription;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisEntryWithoutKeyboardExtendedMessage;

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
