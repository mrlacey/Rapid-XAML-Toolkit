// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddRowAndColumnDefinitionsAction : InjectFixedXamlSuggestedAction
    {
        public AddRowAndColumnDefinitionsAction()
        {
            this.InjectedXaml = @"<Grid.RowDefinitions>
    <RowDefinition Height=""Auto"" />
    <RowDefinition Height=""*"" />
</Grid.RowDefinitions>
<Grid.ColumnDefinitions>
    <ColumnDefinition Width=""*"" />
    <ColumnDefinition Width=""*"" />
</Grid.ColumnDefinitions>";

            this.UndoOperationName = StringRes.Info_UndoContextAddRowAndColumnDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.TwoRowsTwoColumns;

        public override string DisplayText { get; } = "Add RowDefinitions and ColumnDefinitions"; // TODO: localize

        public static AddRowAndColumnDefinitionsAction Create(AddRowAndColumnDefinitionsTag tag)
        {
            var result = new AddRowAndColumnDefinitionsAction
            {
                Tag = tag,
            };

            return result;
        }
    }
}
