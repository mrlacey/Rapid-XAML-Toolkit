// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UseMediaPlayerElementTag : RapidXamlDisplayedTag
    {
        // https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.MediaElement#remarks
        public UseMediaPlayerElementTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, "RXT402", TagErrorType.Warning, logger)
        {
            this.SuggestedAction = typeof(MediaElementAction);
            this.ToolTip = StringRes.Info_XamlAnalysisUseMediaPlayerElementToolTip;
            this.Description = StringRes.Info_XamlAnalysisUseMediaPlayerElementDescription;
            this.ExtendedMessage = StringRes.Info_XamlAnalysisUseMediaPlayerElementExtendedMessage;
        }

        public int InsertPosition { get; set; }
    }
}
