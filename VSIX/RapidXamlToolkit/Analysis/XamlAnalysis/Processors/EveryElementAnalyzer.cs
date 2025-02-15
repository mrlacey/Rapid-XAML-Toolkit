// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EveryElementAnalyzer : BuiltInXamlAnalyzer
    {
        public EveryElementAnalyzer(IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => "Every-Element (Internal)";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var result = this.CheckForHardCodedString(Attributes.TooltipServiceDotToolTip, AttributeType.Inline, element, extraDetails);

            foreach (var attr in element.Attributes)
            {
                if (attr.Name == Attributes.Uid ||
                    attr.Name == Attributes.X_Uid)
                {
                    if (!char.IsUpper(attr.StringValue[0]))
                    {
                        result.Add(
                            AnalysisActions.ReplaceAttributeValue(
                                RapidXamlErrorType.Warning,
                                "RXT451",
                                StringRes.UI_XamlAnalysisUidTitleCaseDescription.WithParams(attr.Name),
                                StringRes.UI_CapitalizeFirstLetterOfUid,
                                attr.Name,
                                attr.StringValue.Substring(0, 1).ToUpper() + attr.StringValue.Substring(1)));
                    }
                }
                else if (attr.Name == Attributes.Name ||
                        attr.Name == Attributes.X_Name)
                {
                    if (!char.IsUpper(attr.StringValue[0]))
                    {
                        result.Add(
                            AnalysisActions.ReplaceAttributeValue(
                                RapidXamlErrorType.Warning,
                                "RXT452",
                                StringRes.UI_XamlAnalysisNameTitleCaseDescription.WithParams(attr.Name),
                                StringRes.UI_CapitalizeFirstLetterOfName,
                                attr.Name,
                                attr.StringValue.Substring(0, 1).ToUpper() + attr.StringValue.Substring(1)));
                    }
                }
            }

            return result;
        }
    }
}
