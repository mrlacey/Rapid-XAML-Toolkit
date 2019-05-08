// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class SelectedItemModeTag : RapidXamlDisplayedTag
    {
        public SelectedItemModeTag(Span span, ITextSnapshot snapshot, string fileName, int line, int column)
            : base(span, snapshot, fileName, "RXT160", line, column, TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(SelectedItemModeAction);
            this.ToolTip = StringRes.Info_XamlAnalysisSetBindingModeToTwoWayToolTip;
            this.Description = StringRes.Info_XamlAnalysisSetBindingModeToTwoWayDescription;
        }

        public int InsertPosition { get; set; }

        public string ExistingBindingMode { get; set; }
    }
}
