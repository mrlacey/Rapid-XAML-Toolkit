// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddRowAndColumnDefinitionsTag : InsertionTag
    {
        public AddRowAndColumnDefinitionsTag(Span span, ITextSnapshot snapshot, string fileName)
            : base(span, snapshot, fileName)
        {
            this.SuggestedAction = typeof(AddRowAndColumnDefinitionsAction);
        }
    }
}
