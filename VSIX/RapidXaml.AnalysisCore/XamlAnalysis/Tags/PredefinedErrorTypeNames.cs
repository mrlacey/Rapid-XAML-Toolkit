// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    // This was copied from Microsoft.VisualStudio.Text.Adornments in Microsoft.VisualStudio.Text.UI
    // So as to help break the direct dependency on VSSDK
    public static class PredefinedErrorTypeNames
    {
        public const string SyntaxError = "syntax error";
        public const string CompilerError = "compiler error";
        public const string OtherError = "other error";
        public const string Warning = "compiler warning";
        public const string Suggestion = "suggestion";
        public const string HintedSuggestion = "hinted suggestion";
    }
}
