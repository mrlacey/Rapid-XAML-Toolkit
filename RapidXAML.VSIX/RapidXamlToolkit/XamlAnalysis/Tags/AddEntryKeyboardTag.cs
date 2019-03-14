// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddEntryKeyboardTag : RapidXamlSuggestionTag
    {
        public AddEntryKeyboardTag(Span span, ITextSnapshot snapshot, int line, int column)
            : base(span, snapshot, "RXT300", line, column)
        {
            this.SuggestedAction = typeof(AddEntryKeyboardAction);
            this.Description = StringRes.Info_XamlAnalysisEntryWithoutKeyboardDescription;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisEntryWithoutKeyboardExtendedMessage;
        }

        public int InsertPosition { get; set; }
    }
}
