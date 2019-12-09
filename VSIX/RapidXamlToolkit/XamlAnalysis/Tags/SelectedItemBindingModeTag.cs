// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class SelectedItemBindingModeTag : RapidXamlDisplayedTag
    {
        public SelectedItemBindingModeTag(Span span, ITextSnapshot snapshot, string fileName)
            : base(span, snapshot, fileName, "RXT160", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(SelectedItemBindingModeAction);
            this.ToolTip = StringRes.Info_XamlAnalysisSetBindingModeToTwoWayToolTip;
            this.Description = StringRes.Info_XamlAnalysisSetBindingModeToTwoWayDescription;
        }

        public int InsertPosition { get; set; }

        public string ExistingBindingMode { get; set; }
    }
}
