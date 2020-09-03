// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Adornments;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public static class TagErrorTypeExtensions
    {
        public static string AsVsAdornmentErrorType(this TagErrorType source)
        {
            switch (source)
            {
                case TagErrorType.Error:
                    return PredefinedErrorTypeNames.OtherError;

                case TagErrorType.Suggestion:
                    return PredefinedErrorTypeNames.Suggestion;

                case TagErrorType.Hidden:
                    // Need to return something that won't cause a problem for the editor.
                    // Shouldn't be creating tags for things that are hidden so shouldn't ever get here.
                    return PredefinedErrorTypeNames.HintedSuggestion;

                case TagErrorType.Warning:
                default:
                    return PredefinedErrorTypeNames.Warning;
            }
        }
    }
}
