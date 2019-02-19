// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class GridProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
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

            var leftPad = linePadding.Contains("\t") ? linePadding + "\t" : linePadding + "    ";

            if (!hasRowDef)
            {
                var tag = new AddRowDefinitionsTag(new Span(offset, endOfOpening), snapshot)
                {
                    InsertPosition = offset + endOfOpening,
                    LeftPad = leftPad,
                };
                tags.Add(tag);
            }

            if (!hasColDef)
            {
                var tag = new AddColumnDefinitionsTag(new Span(offset, endOfOpening), snapshot)
                {
                    InsertPosition = offset + endOfOpening,
                    LeftPad = leftPad,
                };
                tags.Add(tag);
            }

            if (!hasRowDef && !hasColDef)
            {
                var tag = new AddRowAndColumnDefinitionsTag(new Span(offset, endOfOpening), snapshot)
                {
                    InsertPosition = offset + endOfOpening,
                    LeftPad = leftPad,
                };
                tags.Add(tag);
            }

            const string rowDefStart = "<RowDefinition";

            var rowDefsCount = 0;

            var toAdd = new List<InsertRowDefinitionTag>();

            var rowDefIndex = xamlElement.IndexOf(rowDefStart, StringComparison.Ordinal);

            while (rowDefIndex >= 0)
            {
                var endPos = xamlElement.IndexOf('>', rowDefIndex);

                var tag = new InsertRowDefinitionTag(new Span(offset + rowDefIndex, endPos - rowDefIndex + 1), snapshot)
                {
                    RowId = rowDefsCount,
                    GridStartPos = offset,
                    GridLength = xamlElement.Length,
                    XamlTag = xamlElement.Substring(rowDefIndex, endPos - rowDefIndex + 1),
                    InsertPoint = offset + rowDefIndex,
                };

                rowDefsCount += 1;

                toAdd.Add(tag);

                rowDefIndex = xamlElement.IndexOf(rowDefStart, endPos, StringComparison.Ordinal);
            }

            foreach (var tag in toAdd)
            {
                tag.RowCount = rowDefsCount;
                tags.Add(tag);
            }

            const string colDef = "<ColumnDefinition";

            var colDefsCount = 0;
            var colDefOffset = 0;

            var colDefIndex = xamlElement.IndexOf(colDef, StringComparison.Ordinal);

            while (colDefIndex > -1)
            {
                colDefOffset += colDefIndex + colDef.Length;
                colDefsCount += 1;

                colDefIndex = xamlElement.IndexOf(colDef, colDefOffset, StringComparison.Ordinal);
            }

            const string rowDefUse = "Grid.Row=\"";
            const string colDefUse = "Grid.Column=\"";

            int highestAssignedRow = -1;
            int highestAssignedCol = -1;

            var undefinedTags = new List<MissingDefinitionTag>();

            var nextDefUseIndex = xamlElement.FirstIndexOf(rowDefUse, colDefUse);
            var defUseOffset = 0;

            while (nextDefUseIndex >= 0)
            {
                defUseOffset += nextDefUseIndex;

                var line = snapshot.GetLineFromPosition(offset + defUseOffset);
                var col = offset + defUseOffset - line.Start.Position;

                // Get assigned value
                if (xamlElement.Substring(defUseOffset).StartsWith(rowDefUse))
                {
                    var valueStartPos = defUseOffset + rowDefUse.Length;
                    var closePos = xamlElement.IndexOf("\"", valueStartPos, StringComparison.Ordinal);

                    var assignedStr = xamlElement.Substring(valueStartPos, closePos - valueStartPos);

                    if (int.TryParse(assignedStr, out int assignedInt))
                    {
                        if (assignedInt >= rowDefsCount)
                        {
                            undefinedTags.Add(new MissingRowDefinitionTag(
                                new Span(offset + defUseOffset, closePos - defUseOffset + 1),
                                snapshot,
                                line.LineNumber,
                                col)
                            {
                                AssignedInt = assignedInt,
                                // TODO : localize
                                Description = "Use of undefined row {0}".WithParams(assignedInt),
                            });
                        }

                        if (assignedInt > highestAssignedRow)
                        {
                            highestAssignedRow = assignedInt;
                        }
                    }
                }
                else if (xamlElement.Substring(defUseOffset).StartsWith(colDefUse))
                {
                    var valueStartPos = defUseOffset + colDefUse.Length;
                    var closePos = xamlElement.IndexOf("\"", valueStartPos, StringComparison.Ordinal);

                    var assignedStr = xamlElement.Substring(valueStartPos, closePos - valueStartPos);

                    if (int.TryParse(assignedStr, out int assignedInt))
                    {
                        if (assignedInt >= colDefsCount)
                        {
                            undefinedTags.Add(new MissingColumnDefinitionTag(
                                new Span(offset + rowDefIndex, closePos - rowDefIndex + 1),
                                snapshot,
                                line.LineNumber,
                                col)
                            {
                                AssignedInt = assignedInt,
                                // TODO : localize
                                Description = "Use of undefined column {0}".WithParams(assignedInt),
                            });
                        }

                        if (assignedInt > highestAssignedCol)
                        {
                            highestAssignedCol = assignedInt;
                        }
                    }
                }

                nextDefUseIndex = xamlElement.Substring(defUseOffset + 1).FirstIndexOf(colDefUse, rowDefUse);
            }

            foreach (var undefinedTag in undefinedTags)
            {
                undefinedTag.TotalDefsRequired = undefinedTag is MissingRowDefinitionTag ? highestAssignedRow
                                                                                         : highestAssignedCol;
                tags.Add(undefinedTag);
            }
        }
    }
}
