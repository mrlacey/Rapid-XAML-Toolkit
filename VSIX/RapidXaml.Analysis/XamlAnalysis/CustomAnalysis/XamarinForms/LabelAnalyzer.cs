// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class LabelAnalyzer : BuiltInXamlAnalyzer
    {
        public LabelAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.Label;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            return this.CheckForHardCodedString(Attributes.Text, AttributeType.Any, element, extraDetails);

            ////var txtAttr = element.GetAttributes(Attributes.Text).FirstOrDefault();

            ////if (txtAttr != null && txtAttr.HasStringValue)
            ////{
            ////    var value = txtAttr.StringValue;

            ////    // TODO: ISSUE#163 change this to an RXT200 when can handle localization of Xamarin.Forms apps
            ////    if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
            ////    {
            ////        return AnalysisActions.HighlightAttributeWithoutAction(
            ////        errorType: RapidXamlErrorType.Warning,
            ////        code: "RXT201",
            ////        description: StringRes.UI_XamlAnalysisGenericHardCodedStringDescription.WithParams(Elements.Label, Attributes.Text, value),
            ////        attribute: txtAttr);
            ////    }
            ////}

            ////return AnalysisActions.None;
        }
    }
}
