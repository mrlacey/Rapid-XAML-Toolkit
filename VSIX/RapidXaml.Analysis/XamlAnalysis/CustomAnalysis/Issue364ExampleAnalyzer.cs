// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class Issue364ExampleAnalyzer : BuiltInXamlAnalyzer
    {
        public Issue364ExampleAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => "ANYCONTAINING:=\"{Binding";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = AnalysisActions.EmptyList;

            foreach (var attribute in element.Attributes)
            {
                if (attribute.HasStringValue)
                {
                    var attrValue = attribute.StringValue;

                    if (attrValue.StartsWith("{Binding ") && !attrValue.Contains(" Mode="))
                    {
                        result.HighlightAttributeWithoutAction(RapidXamlErrorType.Warning, "ISSUE364", $"Set binding mode for '{attribute.Name}'.", attribute);
                    }
                }
            }

            return result;
        }
    }
}
