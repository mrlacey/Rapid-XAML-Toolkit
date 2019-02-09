// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class RapidXamlWarningTag : ErrorTag
    {
        public RapidXamlWarningTag(string tooltip)
            : base(PredefinedErrorTypeNames.Warning, tooltip)
        {
        }
    }
}
