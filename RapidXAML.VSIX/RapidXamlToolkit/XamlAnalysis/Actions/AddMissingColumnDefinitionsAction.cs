// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddMissingColumnDefinitionsAction : MissingDefinitionsAction
    {
        public AddMissingColumnDefinitionsAction()
        {
            // localize
            this.UndoOperationName = "Add missing column definitions.";
            this.DisplayText = "Assigned column {0} has not been defined.".WithParams(this.Tag.AssignedInt);
        }

        public override ImageMoniker IconMoniker => KnownMonikers.FourthOfFourColumns;

        public static AddMissingRowDefinitionsAction Create(MissingColumnDefinitionTag tag)
        {
            var result = new AddMissingRowDefinitionsAction
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            // TODO: implement add missing column definitions
            // In single undoable action
            // may need to add "<Grid.ColumnDefinitions>"
            // add appropriate number of <ColumnDefinition Width="*" />
            // Force reparse of document - to remove tags that have now been added
        }
    }
}
