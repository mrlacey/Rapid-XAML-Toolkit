// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddTextBoxInputScopeAction : BaseSuggestedAction
    {
        public AddTextBoxInputScopeAction(string file)
            : base(file)
        {
            this.DisplayText = StringRes.UI_AddTextBoxInputScope;
        }

        public AddTextBoxInputScopeTag Tag { get; private set; }

        public static AddTextBoxInputScopeAction Create(AddTextBoxInputScopeTag tag, string file)
        {
            var result = new AddTextBoxInputScopeAction(file)
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextAddTextBoxInputScope);
            try
            {
                var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition) + 1;

                vs.ReplaceInActiveDocOnLine("<TextBox ", "<TextBox InputScope=\"Default\" ", lineNumber);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
