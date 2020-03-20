// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class RapidXamlWarningAdornmentTag : ErrorTag
    {
        public RapidXamlWarningAdornmentTag(string tooltip, string adornmentErrorType)
            : base(adornmentErrorType, tooltip)
        {
        }
    }
}
