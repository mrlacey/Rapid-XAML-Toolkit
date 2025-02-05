// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class BindingToXBindAnalyzer : BuiltInXamlAnalyzer
    {
        public BindingToXBindAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework)
             || framework != ProjectFramework.WinUI)
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
                        result.RemoveAttribute(RapidXamlErrorType.Suggestion, "RXT170", $"Use 'x:Bind' rather than 'Binding' for '{attribute.Name}'.", "Change to 'x:Bind'", attribute)
                              .AndAddAttribute(attribute.Name, attrValue.Replace("{Binding", "{x:Bind"));
                    }
                }
            }

            return result;
        }

        public override string TargetType() => "ANYCONTAINING:=\"{Binding";
    }
}
