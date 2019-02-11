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
        // TODO: add tests for this
        public override void Process(int offset, string xamlElement, ITextSnapshot snapshot, List<IRapidXamlTag> tags)
        {
            const string rowDefStart = "<RowDefinition";

            var count = 0;

            var toAdd = new List<InsertRowDefinitionTag>();

            var rowDefIndex = xamlElement.IndexOf(rowDefStart, StringComparison.Ordinal);

            while (rowDefIndex >= 0)
            {
                var endPos = xamlElement.IndexOf('>', rowDefIndex);

                var tag = new InsertRowDefinitionTag
                {
                    Span = new Span(offset + rowDefIndex, endPos - rowDefIndex + 1),
                    RowId = count,
                    GridStartPos = offset,
                    GridLength = xamlElement.Length,
                    XamlTag = xamlElement.Substring(rowDefIndex, endPos - rowDefIndex + 1),
                    InsertPoint = offset + rowDefIndex,
                };

                count += 1;

                toAdd.Add(tag);

                rowDefIndex = xamlElement.IndexOf(rowDefStart, endPos, StringComparison.Ordinal);
            }

            foreach (var tag in toAdd)
            {
                tag.RowCount = count;
                tags.Add(tag);
            }
        }
    }
}
