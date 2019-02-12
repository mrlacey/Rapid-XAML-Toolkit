// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    // TODO: remove and replace with an actual useful tag - this is just for testing suggestionTags until have an actual one
    public class OtherHardCodedStringTag : RapidXamlSuggestionTag
    {
        // TODO Need to assign a proper error code
        public OtherHardCodedStringTag(Span span, ITextSnapshot snapshot, int line, int column)
            : base(span, snapshot, "RXT???", line, column)
        {
            this.SuggestedAction = typeof(OtherHardCodedStringAction);
            this.ToolTip = "Hard coded string"; // TODO: localize
            this.ExtendedMessage = "Do not use hard coded values. Use a localized resource instead."; // TODO: localize
        }
    }
}
