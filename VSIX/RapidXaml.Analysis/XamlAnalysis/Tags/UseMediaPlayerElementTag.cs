// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UseMediaPlayerElementTag : RapidXamlDisplayedTag
    {
        // https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.MediaElement#remarks
        public UseMediaPlayerElementTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT402", TagErrorType.Warning)
        {
            this.ToolTip = StringRes.UI_XamlAnalysisUseMediaPlayerElementToolTip;
            this.Description = StringRes.UI_XamlAnalysisUseMediaPlayerElementDescription;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisUseMediaPlayerElementExtendedMessage;
        }

        public int InsertPosition { get; set; }
    }
}
