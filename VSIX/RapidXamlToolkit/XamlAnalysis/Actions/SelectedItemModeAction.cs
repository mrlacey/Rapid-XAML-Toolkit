// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class SelectedItemModeAction : BaseSuggestedAction
    {
        public SelectedItemModeAction(string file)
            : base(file)
        {
            this.DisplayText = StringRes.UI_SetBindingModeToTwoWay;
        }

        public SelectedItemModeTag Tag { get; private set; }

        public static SelectedItemModeAction Create(SelectedItemModeTag tag, string file)
        {
            var result = new SelectedItemModeAction(file)
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.UI_SetBindingModeToTwoWay);
            try
            {
                var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition) + 1;

                if (!string.IsNullOrEmpty(this.Tag.ExistingBindingMode))
                {
                    vs.ReplaceInActiveDocOnLine(this.Tag.ExistingBindingMode, "Mode=TwoWay", lineNumber);
                }
                else
                {
                    vs.ReplaceInActiveDocOnLine("}", ", Mode=TwoWay}", lineNumber);
                }

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
