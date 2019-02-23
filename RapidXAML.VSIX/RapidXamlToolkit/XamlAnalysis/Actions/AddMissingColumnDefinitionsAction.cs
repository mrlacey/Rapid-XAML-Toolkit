// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddMissingColumnDefinitionsAction : MissingDefinitionsAction
    {
        public AddMissingColumnDefinitionsAction(string file)
            : base(file)
        {
            this.UndoOperationName = StringRes.Info_UndoContextAddMissingColumnDefinitions;
            this.DisplayText = StringRes.UI_AddMissingColumnDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.FourthOfFourColumns;

        public static AddMissingColumnDefinitionsAction Create(MissingColumnDefinitionTag tag, string file)
        {
            var result = new AddMissingColumnDefinitionsAction(file)
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextAddColumnDefinitions);
            try
            {
                // TODO: may need to add "<Grid.ColumnDefinitions>" - can this be combined with AddColumnDefinitionsAction ?

                const string def = "<ColumnDefinition Width=\"*\" />";

                var insert = string.Empty;

                // TODO: add line wrapping and left padding
                for (var i = 0; i <= this.Tag.TotalDefsRequired - this.Tag.ExistingDefsCount; i++)
                {
                    insert += def;
                }

                vs.InsertAtEndOfLine(this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition), insert);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
