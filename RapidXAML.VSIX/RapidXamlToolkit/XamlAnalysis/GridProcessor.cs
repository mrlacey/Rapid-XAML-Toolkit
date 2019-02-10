// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class GridProcessor : XamlElementProcessor
    {
        // TODO: also need to add all other grid related tag creation
        public override void Process(int offset, string xamlElement, ITextSnapshot snapshot, List<IRapidXamlTag> tags)
        {
            const string rowDefStart = "<RowDefinition";

            var count = 0;

            var rowDefIndex = xamlElement.IndexOf(rowDefStart, StringComparison.Ordinal);
            while (rowDefIndex >= 0)
            {
                var endPos = xamlElement.IndexOf('>', rowDefIndex);

                // TODO: need to cache these until know the total number as need to pass that too.
                var tag = new InsertRowDefinitionTag
                {
                    Span = new Span(offset + rowDefIndex, endPos - rowDefIndex + 1),
                    RowId = count,
                };

                tags.Add(tag);

                count += 1;

                rowDefIndex = xamlElement.IndexOf(rowDefStart, endPos, StringComparison.Ordinal);
            }
        }
    }
}
