// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class RapidXamlSuggestionAdornmentTag : ErrorTag
    {
        public RapidXamlSuggestionAdornmentTag(string tooltip)
            : base(PredefinedErrorTypeNames.HintedSuggestion, tooltip)
        {
        }
    }
}
