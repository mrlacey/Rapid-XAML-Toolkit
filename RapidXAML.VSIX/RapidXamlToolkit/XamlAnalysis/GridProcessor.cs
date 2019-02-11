// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class GridProcessor : XamlElementProcessor
    {
        // TODO: also need to add all other grid related tag creation
        // TODO: add tests for this
        public override void Process(int offset, string xamlElement, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            const string gridOpenSpace = "<Grid ";
            const string gridOpenComplete = "<Grid>";

            var endOfOpening = xamlElement.IndexOf(">", StringComparison.Ordinal) + 1;
            var firstNestedGrid = xamlElement.FirstIndexOf(gridOpenSpace, gridOpenComplete);

            var rowDefPos = xamlElement.IndexOf("<Grid.RowDefinitions", StringComparison.Ordinal);
            var colDefPos = xamlElement.IndexOf("<Grid.ColumnDefinitions", StringComparison.Ordinal);

            var hasRowDef = false;
            if (rowDefPos > 0)
            {
                hasRowDef = firstNestedGrid <= 0 || rowDefPos < firstNestedGrid;
            }

            var hasColDef = false;
            if (colDefPos > 0)
            {
                hasColDef = firstNestedGrid <= 0 || colDefPos < firstNestedGrid;
            }

            if (!hasRowDef)
            {
                var tag = new AddRowDefinitionsTag(new Span(offset, endOfOpening), snapshot)
                {
                    InsertLine = snapshot.GetLineNumberFromPosition(offset + endOfOpening) + 1,
                };
                tags.Add(tag);
            }

            if (!hasColDef)
            {
                var tag = new AddColumnDefinitionsTag(new Span(offset, endOfOpening), snapshot)
                {
                    InsertLine = snapshot.GetLineNumberFromPosition(offset + endOfOpening) + 1,
                };
                tags.Add(tag);
            }

            if (!hasRowDef && !hasColDef)
            {
                var tag = new AddRowAndColumnDefinitionsTag(new Span(offset, endOfOpening), snapshot)
                {
                    InsertLine = snapshot.GetLineNumberFromPosition(offset + endOfOpening) + 1,
                };
                tags.Add(tag);
            }

            const string rowDefStart = "<RowDefinition";

            var count = 0;

            var toAdd = new List<InsertRowDefinitionTag>();

            var rowDefIndex = xamlElement.IndexOf(rowDefStart, StringComparison.Ordinal);

            while (rowDefIndex >= 0)
            {
                var endPos = xamlElement.IndexOf('>', rowDefIndex);

                var tag = new InsertRowDefinitionTag(new Span(offset + rowDefIndex, endPos - rowDefIndex + 1), snapshot)
                {
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
