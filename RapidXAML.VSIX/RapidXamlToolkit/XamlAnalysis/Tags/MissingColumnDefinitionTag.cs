// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class MissingColumnDefinitionTag : MissingDefinitionTag
    {
        // TODO: Need to assign a proper error code
        public MissingColumnDefinitionTag(Span span, ITextSnapshot snapshot, int line, int column)
            : base(span, snapshot, "RXT???", line, column)
        {
            this.SuggestedAction = typeof(AddMissingColumnDefinitionsAction);
            // TODO: Localize
            this.ToolTip = "No corresponding column definition";
            this.ExtendedMessage = "The use of undefined columns can lead to unexpected layout behavior. It can also be confusing for other people looking at the code.";
        }
    }
}
