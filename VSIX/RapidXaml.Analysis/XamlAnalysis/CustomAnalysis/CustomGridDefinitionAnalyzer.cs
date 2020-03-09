// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

#if DEBUG
using System.Linq;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class CustomGridDefinitionAnalyzer : RapidXaml.ICustomAnalyzer
    {
        public string TargetType() => "Grid";

        // Note. Not handled: Col & Row defs of different formats
        // Also, should not return an option to switch to the shorter format if using Min/Max Wpdth/Height properties.
        public AnalysisActions Analyze(RapidXamlElement element)
        {
            var colDef = element.GetAttributes("ColumnDefinitions")?.FirstOrDefault();
            var rowDef = element.GetAttributes("RowDefinitions")?.FirstOrDefault();

            bool isOldFormat = true;

            var newColDefXaml = new System.Text.StringBuilder();
            var newRowDefXaml = new System.Text.StringBuilder();

            if (colDef != null)
            {
                if (colDef.HasStringValue)
                {
                    isOldFormat = false;

                    var shortValue = colDef.StringValue;

                    var colValues = shortValue.Split(',');

                    newColDefXaml.AppendLine("<Grid.ColumnDefinitions>");
                    foreach (var colValue in colValues)
                    {
                        newColDefXaml.AppendLine($"<ColumnDefinition Width=\"{colValue.Trim()}\" />");
                    }

                    newColDefXaml.AppendLine("</Grid.ColumnDefinitions>");
                }
                else
                {
                    foreach (var oldColDef in colDef.Children)
                    {
                        if (oldColDef.Name == "ColumnDefinition")
                        {
                            // Allow for width being optional and defaulting to Star
                            var width = oldColDef.GetAttributes("Width")?.First()?.StringValue ?? "*";

                            if (newColDefXaml.Length > 0)
                            {
                                newColDefXaml.Append(", ");
                            }

                            newColDefXaml.Append(width);
                        }
                    }
                }
            }

            if (rowDef != null)
            {
                if (rowDef.HasStringValue)
                {
                    isOldFormat = false;

                    var shortValue = rowDef.StringValue;

                    var rowValues = shortValue.Split(',');

                    newRowDefXaml.AppendLine("<Grid.RowDefinitions>");
                    foreach (var rowValue in rowValues)
                    {
                        newRowDefXaml.AppendLine($"<RowDefinition Height=\"{rowValue.Trim()}\" />");
                    }

                    newRowDefXaml.AppendLine("</Grid.RowDefinitions>");
                }
                else
                {
                    foreach (var oldRowDef in rowDef.Children)
                    {
                        if (oldRowDef.Name == "RowDefinition")
                        {
                            // Allow for width being optional and defaulting to Star
                            var height = oldRowDef.GetAttributes("Height")?.First()?.StringValue ?? "*";

                            if (newRowDefXaml.Length > 0)
                            {
                                newRowDefXaml.Append(", ");
                            }

                            newRowDefXaml.Append(height);
                        }
                    }
                }
            }

            if (colDef != null || rowDef != null)
            {
                if (isOldFormat)
                {
                    // Offer option to use shorter (new) syntax.
                    return AnalysisActions
                        .RemoveAttribute(
                            RapidXamlErrorType.Warning,
                            "WINUI673A",
                            description: "Can change to other syntax.",
                            actionText: "Use shorter definition syntax",
                            attribute: colDef,
                            moreInfoUrl: "https://github.com/microsoft/microsoft-ui-xaml/issues/673")
                        .AndAddAttribute("ColumnDefinitions", newColDefXaml.ToString().TrimEnd(',', ' '))
                        .AndRemoveAttribute(rowDef)
                        .AndAddAttribute("RowDefinitions", newRowDefXaml.ToString().TrimEnd(',', ' '));
                }
                else
                {
                    // Offer option to use longer (old) syntax.
                    return AnalysisActions
                        .RemoveAttribute(
                            RapidXamlErrorType.Warning,
                            "WINUI673B",
                            description: "Can change to other syntax.",
                            actionText: "Use old definition syntax",
                            attribute: colDef,
                            moreInfoUrl: "https://github.com/microsoft/microsoft-ui-xaml/issues/673")
                        .AndAddChildString(newColDefXaml.ToString())
                        .AndRemoveAttribute(rowDef)
                        .AndAddChildString(newRowDefXaml.ToString());
                }
            }

            // If not returned an action
            return AnalysisActions.None;
        }
    }
}
#endif
