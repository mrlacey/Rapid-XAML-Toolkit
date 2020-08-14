// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddRowAndColumnDefinitionsTag : InsertionTag
    {
        public AddRowAndColumnDefinitionsTag(Span span, IRapidXamlTextSnapshot snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, logger)
        {
            this.SuggestedAction = typeof(AddRowAndColumnDefinitionsAction);
        }
    }
}
