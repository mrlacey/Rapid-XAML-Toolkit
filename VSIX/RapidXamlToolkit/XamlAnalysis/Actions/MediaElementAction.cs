// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class MediaElementAction : BaseSuggestedAction
    {
        public MediaElementAction(string file)
            : base(file)
        {
            this.DisplayText = StringRes.UI_ChangeToMediaPlayerElement;
        }

        public UseMediaPlayerElementTag Tag { get; private set; }

        public static MediaElementAction Create(UseMediaPlayerElementTag tag, string file)
        {
            var result = new MediaElementAction(file)
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.UI_ChangeToMediaPlayerElement);
            try
            {
                var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition) + 1;

                vs.ReplaceInActiveDocOnLine("<MediaElement", "<MediaPlayerElement", lineNumber);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
