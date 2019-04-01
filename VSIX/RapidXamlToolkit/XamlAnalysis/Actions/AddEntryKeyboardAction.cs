// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddEntryKeyboardAction : BaseSuggestedAction
    {
        public AddEntryKeyboardAction(string file)
            : base(file)
        {
            this.DisplayText = StringRes.UI_AddEntryKeyboard;
        }

        public AddEntryKeyboardTag Tag { get; private set; }

        public static AddEntryKeyboardAction Create(AddEntryKeyboardTag tag, string file)
        {
            var result = new AddEntryKeyboardAction(file)
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextAddEntryKeyboard);
            try
            {
                var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition) + 1;

                vs.ReplaceInActiveDocOnLine("<Entry ", "<Entry Keyboard=\"Default\" ", lineNumber);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
