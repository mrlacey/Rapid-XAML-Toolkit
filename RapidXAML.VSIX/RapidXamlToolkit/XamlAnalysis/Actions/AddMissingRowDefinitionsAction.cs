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
    public class AddMissingRowDefinitionsAction : MissingDefinitionsAction
    {
        public AddMissingRowDefinitionsAction(string file)
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
            // TODO: implement add missing row definitions
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextAddRowDefinitions);
            try
            {
                // may need to add "<Grid.RowDefinitions>"
                // add appropriate number of <RowDefinition Height="*" />
                // Force reparse of document - to remove tags that have now been added
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
