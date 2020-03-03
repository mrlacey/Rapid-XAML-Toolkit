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

        public AnalysisActions Analyze(RapidXamlElement element)
        {
            var colDefs = element.GetAttributes("ColumnDefinitions");

            if (colDefs.Count() == 1)
            {
                var colDef = colDefs.First();

                if (colDef.HasStringValue)
                {
                    var shortValue = colDef.StringValue;

                    var colValues = shortValue.Split(',');

                    var newXaml = new System.Text.StringBuilder();
                    newXaml.AppendLine("<Grid.ColumnDefinitions>");
                    foreach (var colValue in colValues)
                    {
                        newXaml.AppendLine($"<ColumnDefinition Width=\"{colValue.Trim()}\" />");
                    }

                    newXaml.AppendLine("</Grid.ColumnDefinitions>");

                    // Offer option to use longer (old) syntax.
                    return AnalysisActions.RemoveAttribute(
                        RapidXamlErrorType.Warning,
                        "GRD002a",
                        description: "Can change to other syntax.",
                        actionText: "Remove short definition syntax",
                        attribute: colDef).AddChildString(
                        RapidXamlErrorType.Warning,
                        code: "GRD002b",
                        description: "Can change to other syntax.",
                        actionText: "Add expanded definition syntax",
                        xaml: newXaml.ToString());
                }
                else
                {
                    // Offer option to use shorter (new) syntax.
                    var newXaml = new System.Text.StringBuilder();
                    newXaml.Append("ColumnDefinitions=\"");

                    var counter = 0;

                    foreach (var oldColDef in colDef.ElementValue.GetChildren("ColumnDefinition"))
                    {
                        var width = oldColDef.GetAttributes("Width")?.First()?.StringValue;

                        if (width != null)
                        {
                            if (counter > 0)
                            {
                                newXaml.Append(", ");
                            }

                            newXaml.Append(width);

                            counter += 1;
                        }
                    }

                    newXaml.Append("\"");

                    return AnalysisActions.RemoveAttribute(
                        RapidXamlErrorType.Warning,
                        "GRD001a",
                        description: "Can change to other syntax.",
                        actionText: "Remove longer definition syntax",
                        attribute: colDef).AddChildString(
                        RapidXamlErrorType.Warning,
                        code: "GRD001b",
                        description: "Can change to other syntax.",
                        actionText: "Add shorter definition syntax",
                        xaml: newXaml.ToString());
                }
            }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
#endif
