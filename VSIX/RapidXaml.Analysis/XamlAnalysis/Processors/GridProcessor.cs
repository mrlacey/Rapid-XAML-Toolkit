// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    // When change this to be based on BuiltInAnalyzer look to optimize performance
    public class GridProcessor : XamlElementProcessor
    {
        public GridProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xlmns = null)
        {
            const string gridOpenSpace = "<Grid ";
            const string gridOpenComplete = "<Grid>";

            var endOfOpening = xamlElement.IndexOf(">", StringComparison.Ordinal) + 1;
            var firstNestedGrid = xamlElement.AsSpan().FirstIndexOf(gridOpenSpace, gridOpenComplete);

            var rowDefPos = xamlElement.IndexOf("<Grid.RowDefinitions", StringComparison.Ordinal);
            var colDefPos = xamlElement.IndexOf("<Grid.ColumnDefinitions", StringComparison.Ordinal);

            var gridIsSelfClosing = XamlElementProcessor.IsSelfClosing(xamlElement.AsSpan());

            var hasRowDef = false;
            var shortRowSyntax = false;
            if (rowDefPos > 0)
            {
                hasRowDef = firstNestedGrid <= 0 || rowDefPos < firstNestedGrid;
            }

            string rowDefsString = null;

            if (rowDefPos < 0 && this.ProjectType.Matches(ProjectType.XamarinForms))
            {
                // See if using new inline format
                if (this.TryGetAttribute(xamlElement, Attributes.RowDefinitions, AttributeType.Inline, out _, out _, out _, out rowDefsString))
                {
                    hasRowDef = true;
                    shortRowSyntax = true;
                }
            }

            var hasColDef = false;
            var shortColSyntax = false;
            if (colDefPos > 0)
            {
                hasColDef = firstNestedGrid <= 0 || colDefPos < firstNestedGrid;
            }

            string colDefsString = null;

            if (colDefPos < 0 && this.ProjectType.Matches(ProjectType.XamarinForms))
            {
                // See if using new inline format
                if (this.TryGetAttribute(xamlElement, Attributes.ColumnDefinitions, AttributeType.Inline, out _, out _, out _, out colDefsString))
                {
                    hasColDef = true;
                    shortColSyntax = true;
                }
            }

            var leftPad = linePadding.Contains("\t") ? linePadding + "\t" : linePadding + "    ";

            // Set to make it clear what the default is.
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var rowDefsClosingPos = -1;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            if (!hasRowDef)
            {
                var tag = new AddRowDefinitionsTag(new Span(offset, endOfOpening), snapshot, fileName, this.Logger)
                {
                    InsertPosition = offset + endOfOpening,
                    LeftPad = leftPad,
                    GridNeedsExpanding = gridIsSelfClosing,
                };
                tags.TryAdd(tag, xamlElement, suppressions);

                rowDefsClosingPos = xamlElement.IndexOf(">", StringComparison.Ordinal);
            }
            else
            {
                rowDefsClosingPos = xamlElement.IndexOf("</Grid.RowDefinitions", StringComparison.Ordinal);
            }

            // Set to make it clear what the default is.
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var colDefsClosingPos = -1;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            if (!hasColDef)
            {
                var tag = new AddColumnDefinitionsTag(new Span(offset, endOfOpening), snapshot, fileName, this.Logger)
                {
                    InsertPosition = offset + endOfOpening,
                    LeftPad = leftPad,
                    GridNeedsExpanding = gridIsSelfClosing,
                };
                tags.TryAdd(tag, xamlElement, suppressions);

                colDefsClosingPos = xamlElement.IndexOf(">", StringComparison.Ordinal);
            }
            else
            {
                colDefsClosingPos = xamlElement.IndexOf("</Grid.ColumnDefinitions", StringComparison.Ordinal);
            }

            if (!hasRowDef && !hasColDef)
            {
                var tag = new AddRowAndColumnDefinitionsTag(new Span(offset, endOfOpening), snapshot, fileName, this.Logger)
                {
                    InsertPosition = offset + endOfOpening,
                    LeftPad = leftPad,
                    GridNeedsExpanding = gridIsSelfClosing,
                };
                tags.TryAdd(tag, xamlElement, suppressions);
            }

            const string rowDefStart = "<RowDefinition";

            var rowDefsCount = 0;

            var toAdd = new List<InsertRowDefinitionTag>();

            var rowDefIndex = xamlElement.IndexOf(rowDefStart, StringComparison.Ordinal);

            while (rowDefIndex >= 0)
            {
                var endPos = xamlElement.IndexOf('>', rowDefIndex);

                var tag = new InsertRowDefinitionTag(new Span(offset + rowDefIndex, endPos - rowDefIndex + 1), snapshot, fileName, this.Logger)
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

            if (rowDefsCount == 0 && !string.IsNullOrEmpty(rowDefsString))
            {
                rowDefsCount = rowDefsString.Split(new[] { ',' }, StringSplitOptions.None).Length;
            }

            foreach (var tag in toAdd)
            {
                tag.RowCount = rowDefsCount;
                tags.TryAdd(tag, xamlElement, suppressions);
            }

            const string colDef = "<ColumnDefinition";

            var colDefsCount = 0;

            var colDefIndex = xamlElement.IndexOf(colDef, StringComparison.Ordinal);

            while (colDefIndex > -1)
            {
                colDefsCount += 1;

                colDefIndex = xamlElement.IndexOf(colDef, colDefIndex + 1, StringComparison.Ordinal);
            }

            if (colDefsCount == 0 && !string.IsNullOrEmpty(colDefsString))
            {
                colDefsCount = colDefsString.Split(new[] { ',' }, StringSplitOptions.None).Length;
            }

            const string rowDefUse = "Grid.Row=\"";
            const string colDefUse = "Grid.Column=\"";

            int highestAssignedRow = -1;
            int highestAssignedCol = -1;

            var undefinedTags = new List<MissingDefinitionTag>();

            var nextDefUseIndex = xamlElement.AsSpan().FirstIndexOf(rowDefUse, colDefUse);
            var defUseOffset = 0;

            while (nextDefUseIndex > 0)
            {
                defUseOffset += nextDefUseIndex;

                if (nextDefUseIndex > endOfOpening)
                {
                    if (!xamlElement.AsSpan().InComment(defUseOffset))
                    {
                        // Get assigned value
                        if (xamlElement.Substring(defUseOffset).StartsWith(rowDefUse))
                        {
                            var valueStartPos = defUseOffset + rowDefUse.Length;
                            var closePos = xamlElement.IndexOf("\"", valueStartPos, StringComparison.Ordinal);

                            var assignedStr = xamlElement.Substring(valueStartPos, closePos - valueStartPos);

                            if (int.TryParse(assignedStr, out int assignedInt))
                            {
                                if (assignedInt > 0 && assignedInt >= rowDefsCount)
                                {
                                    var tagDeps = this.CreateBaseTagDependencies(
                                        new Span(offset + defUseOffset, closePos - defUseOffset + 1),
                                        snapshot,
                                        fileName);

                                    undefinedTags.Add(new MissingRowDefinitionTag(tagDeps)
                                    {
                                        AssignedInt = assignedInt,
                                        Description = StringRes.UI_XamlAnalysisMissingRowDefinitionDescription.WithParams(assignedInt),
                                        ExistingDefsCount = rowDefsCount,
                                        HasSomeDefinitions = hasRowDef,
                                        UsesShortDefinitionSyntax = shortRowSyntax,
                                        InsertPosition = offset + rowDefsClosingPos,
                                        LeftPad = leftPad,
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
                                if (assignedInt > 0 && assignedInt >= colDefsCount)
                                {
                                    var tagDeps = this.CreateBaseTagDependencies(
                                        new Span(offset + defUseOffset, closePos - defUseOffset + 1),
                                        snapshot,
                                        fileName);

                                    undefinedTags.Add(new MissingColumnDefinitionTag(tagDeps)
                                    {
                                        AssignedInt = assignedInt,
                                        Description = StringRes.UI_XamlAnalysisMissingColumnDefinitionDescription.WithParams(assignedInt),
                                        ExistingDefsCount = colDefsCount,
                                        HasSomeDefinitions = hasColDef,
                                        UsesShortDefinitionSyntax = shortColSyntax,
                                        InsertPosition = offset + colDefsClosingPos,
                                        LeftPad = leftPad,
                                    });
                                }

                                if (assignedInt > highestAssignedCol)
                                {
                                    highestAssignedCol = assignedInt;
                                }
                            }
                        }
                    }
                }

                nextDefUseIndex = xamlElement.Substring(defUseOffset + 1).AsSpan().FirstIndexOf(colDefUse, rowDefUse) + 1;
            }

            foreach (var undefinedTag in undefinedTags)
            {
                undefinedTag.TotalDefsRequired = undefinedTag is MissingRowDefinitionTag ? highestAssignedRow
                                                                                         : highestAssignedCol;
                tags.TryAdd(undefinedTag, xamlElement, suppressions);
            }

            const string rowSpanUse = "Grid.RowSpan=\"";
            const string colSpanUse = "Grid.ColumnSpan=\"";

            var nextSpanUseIndex = xamlElement.AsSpan().FirstIndexOf(rowSpanUse, colSpanUse);
            var spanUseOffset = 0;

            while (nextSpanUseIndex > 0)
            {
                spanUseOffset += nextSpanUseIndex;

                if (nextSpanUseIndex > endOfOpening)
                {
                    if (!xamlElement.AsSpan().InComment(spanUseOffset))
                    {
                        if (xamlElement.Substring(spanUseOffset).StartsWith(rowSpanUse))
                        {
                            var valueStartPos = spanUseOffset + rowSpanUse.Length;
                            var closePos = xamlElement.IndexOf("\"", valueStartPos, StringComparison.Ordinal);

                            var assignedStr = xamlElement.Substring(valueStartPos, closePos - valueStartPos);

                            if (int.TryParse(assignedStr, out int assignedInt))
                            {
                                var element = XamlElementProcessor.GetSubElementAtPosition(this.ProjectType, fileName, snapshot, xamlElement, spanUseOffset, this.Logger, this.ProjectFilePath, this.VSPFP);

                                var row = 0;
                                if (this.TryGetAttribute(element, "Grid.Row", AttributeType.InlineOrElement, out _, out _, out _, out string rowStr))
                                {
                                    row = int.Parse(rowStr);
                                }

                                if (assignedInt > 1 && assignedInt - 1 + row >= rowDefsCount)
                                {
                                    var tagDeps = this.CreateBaseTagDependencies(
                                        new Span(offset + spanUseOffset, closePos - spanUseOffset + 1),
                                        snapshot,
                                        fileName);

                                    var rowTag = new RowSpanOverflowTag(tagDeps)
                                    {
                                        TotalDefsRequired = assignedInt + row - 1,
                                        Description = StringRes.UI_XamlAnalysisRowSpanOverflowDescription,
                                        ExistingDefsCount = rowDefsCount,
                                        HasSomeDefinitions = hasRowDef,
                                        InsertPosition = offset + rowDefsClosingPos,
                                        LeftPad = leftPad,
                                    };

                                    tags.TryAdd(rowTag, xamlElement, suppressions);
                                }
                            }
                        }
                        else if (xamlElement.Substring(spanUseOffset).StartsWith(colSpanUse))
                        {
                            var valueStartPos = spanUseOffset + colSpanUse.Length;
                            var closePos = xamlElement.IndexOf("\"", valueStartPos, StringComparison.Ordinal);

                            var assignedStr = xamlElement.Substring(valueStartPos, closePos - valueStartPos);

                            if (int.TryParse(assignedStr, out int assignedInt))
                            {
                                var element = XamlElementProcessor.GetSubElementAtPosition(this.ProjectType, fileName, snapshot, xamlElement, spanUseOffset, this.Logger, this.ProjectFilePath, this.VSPFP);

                                var gridCol = 0;
                                if (this.TryGetAttribute(element, "Grid.Column", AttributeType.InlineOrElement, out _, out _, out _, out string colStr))
                                {
                                    gridCol = int.Parse(colStr);
                                }

                                if (assignedInt > 1 && assignedInt - 1 + gridCol >= colDefsCount)
                                {
                                    var tagDeps = this.CreateBaseTagDependencies(
                                        new Span(offset + spanUseOffset, closePos - spanUseOffset + 1),
                                        snapshot,
                                        fileName);

                                    var colTag = new ColumnSpanOverflowTag(tagDeps)
                                    {
                                        TotalDefsRequired = assignedInt - 1 + gridCol,
                                        Description = StringRes.UI_XamlAnalysisColumnSpanOverflowDescription,
                                        ExistingDefsCount = colDefsCount,
                                        HasSomeDefinitions = hasColDef,
                                        InsertPosition = offset + colDefsClosingPos,
                                        LeftPad = leftPad,
                                    };

                                    tags.TryAdd(colTag, xamlElement, suppressions);
                                }
                            }
                        }
                    }
                }

                nextSpanUseIndex = xamlElement.Substring(spanUseOffset + 1).AsSpan().FirstIndexOf(colSpanUse, rowSpanUse) + 1;
            }
        }
    }
}
