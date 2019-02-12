// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    // TODO: remove and replace with an actual useful tag - this is just for testing suggestionTags until have an actual one
    public class OtherHardCodedStringTag : RapidXamlSuggestionTag
    {
        public OtherHardCodedStringTag(Span span, ITextSnapshot snapshot)
            : base(span, snapshot)
        {
            this.SuggestedAction = typeof(OtherHardCodedStringAction);
            this.ToolTip = "SUGGESTION HardCoded string message"; // TODO: need to customize this???
            this.ErrorCode = "RXT???"; // TODO Need to assign a proper error code
        }
    }
}
