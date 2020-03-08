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
            // TODO: support RowDefinitions too
            var colDefs = element.GetAttributes("ColumnDefinitions");
            var rowDefs = element.GetAttributes("RowDefinitions");

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
                        actionText: "Use old definition syntax",
                        attribute: colDef,
                        moreInfoUrl: "https://github.com/microsoft/microsoft-ui-xaml/issues/673")
                        .AndAddChildString(newXaml.ToString());
                }
                else
                {
                    // Offer option to use shorter (new) syntax.
                    var newXaml = new System.Text.StringBuilder();

                    foreach (var oldColDef in colDef.Children.Where(c => c.Name == "ColumnDefinition"))
                    {
                        // Allow for width being optional and defaulting to Star
                        var width = oldColDef.GetAttributes("Width")?.First()?.StringValue ?? "*";

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
                        actionText: "Use shorter definition syntax",
                        attribute: colDef,
                        moreInfoUrl: "https://github.com/microsoft/microsoft-ui-xaml/issues/673")
                        .AndAddAttribute("ColumnDefinitions", newXaml.ToString().TrimEnd(',', ' '));
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
