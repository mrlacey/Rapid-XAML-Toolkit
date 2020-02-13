// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class FooAnalysis : RapidXaml.CustomAnalysis
    {
        public override string TargetType() => "Foo";

        public override AnalysisActions Analyze(RapidXamlElement element)
        {
            if (!element.ContainsAttribute("Bar"))
            {
                return AnalysisActions.AddAttribute(
                    RapidXamlErrorType.Warning,
                    code: "RXT888",
                    description: "Always specify the 'Bar' attribute",
                    actionText: "Add missing 'Bar' attribute",
                    addAttributeName: "Bar",
                    addAttributeValue: "TODO-SET-THIS");
            }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
