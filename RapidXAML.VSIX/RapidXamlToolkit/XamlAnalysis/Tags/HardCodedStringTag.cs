// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class HardCodedStringTag : RapidXamlWarningTag
    {
        // TODO Need to assign a proper error code
        public HardCodedStringTag(Span span, ITextSnapshot snapshot, int line, int column)
            : base(span, snapshot, "RXT???", line, column)
        {
            this.SuggestedAction = typeof(HardCodedStringAction);
            this.ToolTip = "HardCoded string message"; // TODO: need to customize this???
        }
    }
}
