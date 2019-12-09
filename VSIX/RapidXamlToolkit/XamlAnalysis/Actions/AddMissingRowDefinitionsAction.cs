// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddMissingRowDefinitionsAction : MissingDefinitionsAction
    {
        protected AddMissingRowDefinitionsAction(string file)
            : base(file)
        {
            this.UndoOperationName = StringRes.Info_UndoContextAddMissingRowDefinitions;
            this.DisplayText = StringRes.UI_AddMissingRowDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.FourthOfFourRows;

        public static AddMissingRowDefinitionsAction Create(MissingRowDefinitionTag tag, string file)
        {
            var result = new AddMissingRowDefinitionsAction(file)
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextAddRowDefinitions);

            try
            {
                var insert = string.Empty;

                const string def = "<RowDefinition Height=\"*\" />";

                var leftPad = this.Tag.LeftPad.Contains("\t") ? this.Tag.LeftPad + "\t" : this.Tag.LeftPad + "    ";

                for (var i = 0; i <= this.Tag.TotalDefsRequired - this.Tag.ExistingDefsCount; i++)
                {
                    insert += $"{Environment.NewLine}{leftPad}{def}";
                }

                var insertLine = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition);

                if (!this.Tag.HasSomeDefinitions)
                {
                    insert = $"{Environment.NewLine}{this.Tag.LeftPad}<Grid.RowDefinitions>{insert}{Environment.NewLine}{this.Tag.LeftPad}</Grid.RowDefinitions>";

                    // Account for different reference position - end-of-start vs start-of-end
                    insertLine += 1;
                }

                vs.InsertAtEndOfLine(insertLine, insert);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
