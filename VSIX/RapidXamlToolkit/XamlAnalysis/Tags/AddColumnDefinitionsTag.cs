// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddColumnDefinitionsTag : InsertionTag
    {
        public AddColumnDefinitionsTag(Span span, ITextSnapshot snapshot, string fileName)
            : base(span, snapshot, fileName)
        {
            this.SuggestedAction = typeof(AddColumnDefinitionsAction);
        }
    }
}
