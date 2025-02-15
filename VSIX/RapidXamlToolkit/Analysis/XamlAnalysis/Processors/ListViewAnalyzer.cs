// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class ListViewAnalyzer : BuiltInXamlAnalyzer
    {
        public ListViewAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.ListView;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.IsFramework(ProjectFramework.Uwp) && !extraDetails.IsFramework(ProjectFramework.WinUI))
            {
                return AnalysisActions.None;
            }

            if (element.TryGetAttributeStringValue(Attributes.SelectedItem, out string value))
            {
                if (value.StartsWith("{") && !value.Contains("TwoWay"))
                {
                    const string oneTime = "Mode=OneTime";
                    const string oneWay = "Mode=OneWay";

                    string newValue = string.Empty;

                    if (value.Contains(oneTime))
                    {
                        newValue = value.Replace(oneTime, "Mode=TwoWay");
                    }
                    else if (value.Contains(oneWay))
                    {
                        newValue = value.Replace(oneWay, "Mode=TwoWay");
                    }

                    if (!string.IsNullOrEmpty(newValue))
                    {
                        return AnalysisActions.ReplaceAttributeValue(
                            RapidXamlErrorType.Warning,
                            "RXT160",
                            StringRes.UI_XamlAnalysisSetBindingModeToTwoWayDescription,
                            StringRes.UI_SetBindingModeToTwoWay,
                            Attributes.SelectedItem,
                            newValue);
                    }
                }
            }

            return AnalysisActions.None;
        }
    }
}
