using System;
using RapidXaml;

namespace $ext_safeprojectname$
{
    public class MyElementAnalyzer : ICustomAnalyzer
    {
        // TODO: set the name of the element/type this analyzer will analyze.
        public string TargetType() => "MyElement";

        public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            // TODO: Implement this analyzer as per your needs.
            // More details at https://github.com/mrlacey/Rapid-XAML-Toolkit/blob/main/docs/custom-analysis.md
            if (element.ContainsAttribute("IsEnabled"))
            {
                return AnalysisActions.None;
            }
            else
            {
                return AnalysisActions.AddAttribute(
                    errorType: RapidXamlErrorType.Warning,
                    code: "XMPL01",
                    description: "Always set the 'IsEnabled' property on 'MyElement'.",
                    actionText: "Add 'IsEnabled' attribute",
                    addAttributeName: "IsEnabled",
                    addAttributeValue: "True"
                    );
            }
        }
    }
}
