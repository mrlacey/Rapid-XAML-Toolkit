// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddMissingRowDefinitionsAction : MissingDefinitionsAction
    {
        public AddMissingRowDefinitionsAction()
        {
            this.UndoOperationName = StringRes.Info_UndoContextAddMissingRowDefinitions;
            this.DisplayText = StringRes.UI_AddMissingRowDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.FourthOfFourRows;

        public static AddMissingRowDefinitionsAction Create(MissingRowDefinitionTag tag)
        {
            var result = new AddMissingRowDefinitionsAction
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            // TODO: implement add missing row definitions
            // In single undoable action
            // may need to add "<Grid.RowDefinitions>"
            // add appropriate number of <RowDefinition Height="*" />
            // Force reparse of document - to remove tags that have now been added
        }
    }
}
