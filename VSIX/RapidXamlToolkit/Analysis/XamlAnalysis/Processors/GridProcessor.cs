// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    // TODO: Rename file when finished moving logic from the processor to the analyzer
    public class GridAnalyzer : BuiltInXamlAnalyzer
    {
        public GridAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.Grid;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = AnalysisActions.EmptyList;

            int colDefCount = 0;
            int rowDefCount = 0;

            int colHighestUsed = 0;
            int rowHighestUsed = 0;

            var colDefs = element.GetAttributes(Attributes.ColumnDefinitions);

            if (colDefs.Any())
            {
                var colDefOfInterest = colDefs.First();

                if (colDefOfInterest.HasStringValue)
                {
                    colDefCount = colDefs.First().StringValue.Split(',').Count();
                }
                else
                {
                    colDefCount = colDefOfInterest.Children.Count;
                }
            }

            var rowDefs = element.GetAttributes(Attributes.RowDefinitions);

            if (rowDefs.Any())
            {
                var rowDefOfInterest = rowDefs.First();

                if (rowDefOfInterest.HasStringValue)
                {
                    rowDefCount = rowDefs.First().StringValue.Split(',').Count();
                }
                else
                {
                    rowDefCount = rowDefOfInterest.Children.Count;
                }
            }

            foreach (var child in element.Children)
            {
                int row = 0;

                if (child.TryGetAttributeStringValue(Attributes.GridRow, out string gridRow))
                {
                    if (int.TryParse(gridRow, out row))
                    {
                        if (row > rowHighestUsed)
                        {
                            rowHighestUsed = row;
                        }
                    }
                }

                if (child.TryGetAttributeStringValue(Attributes.GridRowSpan, out string rowSpan))
                {
                    if (int.TryParse(rowSpan, out int rspan))
                    {
                        if (row + rspan > rowHighestUsed)
                        {
                            rowHighestUsed = row + rspan;
                        }
                    }
                }

                int col = 0;

                if (child.TryGetAttributeStringValue(Attributes.GridColumn, out string gridCol))
                {
                    if (int.TryParse(gridCol, out col))
                    {
                        if (col > colHighestUsed)
                        {
                            colHighestUsed = col;
                        }
                    }
                }

                if (child.TryGetAttributeStringValue(Attributes.GridColumnSpan, out string colSpan))
                {
                    if (int.TryParse(colSpan, out int cspan))
                    {
                        if (col + cspan > colHighestUsed)
                        {
                            colHighestUsed = col + cspan;
                        }
                    }
                }

                if (int.TryParse(rowSpan, out int irowspan))
                {
                    if (row + irowspan > rowDefCount)
                    {
                        result.Add(AnalysisActions.HighlightDescendantWithoutAction(
                            RapidXamlErrorType.Warning,
                            "RXT103",
                            StringRes.UI_XamlAnalysisRowSpanOverflowDescription,
                            child,
                            extendedMessage: StringRes.UI_XamlAnalysisMissingRowDefinitionExtendedMessage));
                    }
                }

                if (int.TryParse(colSpan, out int icolspan))
                {
                    if (col + icolspan > colDefCount)
                    {
                        result.Add(AnalysisActions.HighlightDescendantWithoutAction(
                            RapidXamlErrorType.Warning,
                            "RXT104",
                            StringRes.UI_XamlAnalysisColumnSpanOverflowDescription,
                            child,
                            extendedMessage: StringRes.UI_XamlAnalysisMissingColumnDefinitionExtendedMessage));
                    }
                }
            }

            if (rowHighestUsed > 0 && rowHighestUsed >= rowDefCount)
            {
                if (rowDefs.Any())
                {
                    if (element.TryGetAttributeStringValue(Attributes.RowDefinitions, out _))
                    {
                        result.Add(AnalysisActions.ReplaceAttributeValue(
                            RapidXamlErrorType.Warning,
                            "RXT101",
                            StringRes.UI_XamlAnalysisMissingRowDefinitionDescription.WithParams(rowHighestUsed),
                            StringRes.UI_AddMissingRowDefinitions,
                            Attributes.RowDefinitions,
                            string.Concat(rowDefs.First().StringValue, string.Concat(Enumerable.Repeat(",*", rowHighestUsed - rowDefCount + 1))),
                            extendedMessage: StringRes.UI_XamlAnalysisMissingRowDefinitionExtendedMessage));
                    }
                    else
                    {
                        result.Add(AnalysisActions.HighlightWithoutAction(
                            RapidXamlErrorType.Warning,
                            "RXT101",
                            StringRes.UI_XamlAnalysisMissingRowDefinitionDescription.WithParams(rowHighestUsed),
                            extendedMessage: StringRes.UI_XamlAnalysisMissingRowDefinitionExtendedMessage));
                    }
                }
                else
                {
                    result.Add(AnalysisActions.AddAttribute(
                        RapidXamlErrorType.Warning,
                        "RXT101",
                        StringRes.UI_XamlAnalysisMissingRowDefinitionDescription.WithParams(rowHighestUsed),
                        StringRes.UI_AddMissingRowDefinitions,
                        Attributes.RowDefinitions,
                        string.Join(",", Enumerable.Repeat('*', rowHighestUsed - rowDefCount + 1)),
                        extendedMessage: StringRes.UI_XamlAnalysisMissingRowDefinitionExtendedMessage));
                }
            }

            if (colHighestUsed > 0 && colHighestUsed >= colDefCount)
            {
                if (colDefs.Any())
                {
                    if (element.TryGetAttributeStringValue(Attributes.ColumnDefinitions, out _))
                    {
                        result.Add(AnalysisActions.ReplaceAttributeValue(
                        RapidXamlErrorType.Warning,
                        "RXT102",
                        StringRes.UI_XamlAnalysisMissingColumnDefinitionDescription.WithParams(colHighestUsed),
                        StringRes.UI_AddMissingColumnDefinitions,
                        Attributes.ColumnDefinitions,
                        string.Concat(colDefs.First().StringValue, string.Concat(Enumerable.Repeat(",*", colHighestUsed - colDefCount + 1))),
                        extendedMessage: StringRes.UI_XamlAnalysisMissingColumnDefinitionExtendedMessage));
                    }
                    else
                    {
                        result.Add(AnalysisActions.HighlightWithoutAction(
                            RapidXamlErrorType.Warning,
                            "RXT102",
                            StringRes.UI_XamlAnalysisMissingColumnDefinitionDescription.WithParams(colHighestUsed),
                            extendedMessage: StringRes.UI_XamlAnalysisMissingColumnDefinitionExtendedMessage));
                    }
                }
                else
                {
                    result.Add(AnalysisActions.AddAttribute(
                    RapidXamlErrorType.Warning,
                    "RXT102",
                    StringRes.UI_XamlAnalysisMissingColumnDefinitionDescription.WithParams(colHighestUsed),
                    StringRes.UI_AddMissingColumnDefinitions,
                    Attributes.ColumnDefinitions,
                    string.Join(",", Enumerable.Repeat('*', colHighestUsed - colDefCount + 1)),
                    extendedMessage: StringRes.UI_XamlAnalysisMissingColumnDefinitionExtendedMessage));
                }
            }

            return result;
        }
    }
}
