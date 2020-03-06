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
                        "WINUI673B",
                        description: "Can change to other syntax.",
                        actionText: "Remove short definition syntax",
                        attribute: colDef).AndAddChildString(newXaml.ToString());
                }
                else
                {
                    // Offer option to use shorter (new) syntax.
                    var newXaml = new System.Text.StringBuilder();

                    foreach (var oldColDef in colDef.ElementValue.GetChildren("ColumnDefinition"))
                    {
                        var width = oldColDef.GetAttributes("Width")?.First()?.StringValue;

                        if (width != null)
                        {
                            newXaml.Append(width);
                            newXaml.Append(", ");
                        }
                    }

                    return AnalysisActions.RemoveAttribute(
                        RapidXamlErrorType.Warning,
                        "WINUI673A",
                        description: "Can change to other syntax.",
                        actionText: "Remove longer definition syntax",
                        attribute: colDef).AndAddAttribute("ColumnDefinitions", newXaml.ToString().TrimEnd(',', ' '));
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
