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
            this.ToolTip = "Hard coded string"; // TODO: localize
            this.ExtendedMessage = "Do not use hard coded values. Use a localized resource instead."; // TODO: localize
        }
    }
}
