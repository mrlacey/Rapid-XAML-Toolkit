// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{

    public class CheckBoxAnalyzer : BuiltInXamlAnalyzer
    {
        public CheckBoxAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => Elements.CheckBox;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet("framework", out ProjectFramework framework)
             || (framework != ProjectFramework.Uwp && framework != ProjectFramework.Wpf))
            {
                return AnalysisActions.None;
            }

            var result = AnalysisActions.EmptyList;

            if (framework.Equals(ProjectFramework.Uwp))
            {
                // TODO: Issue#163 - check for hard-coded string for  Attributes.Content
                // Then remove CheckBoxProcessor, CheckBoxCheckedAndUncheckedEventsTag & MissingCheckBoxEvents
            }

            // If using one event, the recommendation is to use both
            var hasCheckedEvent = element.HasAttribute(Attributes.CheckedEvent);
            var hasuncheckedEvent = element.HasAttribute(Attributes.UncheckedEvent);

            if (hasCheckedEvent && !hasuncheckedEvent)
            {
                var existingCheckedName = element.Attributes.FirstOrDefault(a => a.Name == Attributes.CheckedEvent).StringValue;

                var newEventName = existingCheckedName.ToLowerInvariant().Contains("checked")
                    ? existingCheckedName.Replace("Checked", "UnChecked").Replace("checked", "Unchecked")
                    : "OnCheckBoxUnchecked";

                result.AddAttribute(
                    RapidXamlErrorType.Warning,
                    "RXT401",
                    StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsDescription,
                    StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsToolTip,
                    Attributes.UncheckedEvent,
                    newEventName,
                    StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsExtendedMessage,
                    "https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/checkbox#handle-click-and-checked-events");
            }

            if (!hasCheckedEvent && hasuncheckedEvent)
            {
                var existingUnheckedName = element.Attributes.FirstOrDefault(a => a.Name == Attributes.UncheckedEvent).StringValue;

                var newEventName = existingUnheckedName.ToLowerInvariant().Contains("unchecked")
                    ? existingUnheckedName.Replace("UnChecked", "Checked").Replace("unchecked", "checked")
                    : "OnCheckBoxChecked";

                result.AddAttribute(
                    RapidXamlErrorType.Warning,
                    "RXT401",
                    StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsDescription,
                    StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsToolTip,
                    Attributes.UncheckedEvent,
                    newEventName,
                    StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsExtendedMessage,
                    "https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/checkbox#handle-click-and-checked-events");
            }

            return result;
        }
    }
}
