// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class CheckBoxCheckedAndUncheckedEventsTag : RapidXamlDisplayedTag
    {
        // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/checkbox#handle-click-and-checked-events
        public CheckBoxCheckedAndUncheckedEventsTag(TagDependencies tagDeps, string existingName, bool hasChecked)
            : base(tagDeps, "RXT401", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(MissingCheckBoxEventAction);
            this.ToolTip = StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsToolTip;
            this.Description = StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsDescription;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisCheckBoxCheckedAndUncheckedEventsExtendedMessage;

            this.ExistingIsChecked = hasChecked;
            this.ExistingName = existingName;
        }

        public int InsertPosition { get; set; }

        public bool ExistingIsChecked { get; set; }

        public string ExistingName { get; set; }
    }
}
