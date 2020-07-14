// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class BindingToXBindAnalyzer : NotReallyCustomAnalyzer
    {
        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet("framework", out ProjectFramework framework)
             || framework != ProjectFramework.Uwp)
            {
                return AnalysisActions.None;
            }

            var result = AnalysisActions.EmptyList;

            foreach (var attribute in element.Attributes)
            {
                if (attribute.HasStringValue)
                {
                    var attrValue = attribute.StringValue;

                    if (attrValue.StartsWith("{Binding"))
                    {
                        result.RemoveAttribute(RapidXamlErrorType.Suggestion, "RXTPOC", $"Use 'x:Bind' rather than 'Binding' for '{attribute.Name}'.", "Change to 'x:Bind'", attribute)
                              .AndAddAttribute(attribute.Name, attrValue.Replace("{Binding", "{x:Bind"));
                    }
                }
            }

            return result;
        }

        public override string TargetType() => "ANYCONTAINING:=\"{Binding";
    }
}
