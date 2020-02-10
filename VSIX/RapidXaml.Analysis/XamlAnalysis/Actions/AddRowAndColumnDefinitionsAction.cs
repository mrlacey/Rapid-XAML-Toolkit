// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddRowAndColumnDefinitionsAction : InjectFixedXamlSuggestedAction
    {
        public AddRowAndColumnDefinitionsAction(string file)
            : base(file)
        {
            this.InjectedXaml = @"<Grid.RowDefinitions>
    <RowDefinition Height=""Auto"" />
    <RowDefinition Height=""*"" />
</Grid.RowDefinitions>
<Grid.ColumnDefinitions>
    <ColumnDefinition Width=""*"" />
    <ColumnDefinition Width=""*"" />
</Grid.ColumnDefinitions>";

            this.UndoOperationName = StringRes.UI_UndoContextAddRowAndColumnDefinitions;
            this.DisplayText = StringRes.UI_AddRowAndColumnDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.TwoRowsTwoColumns;

        public static AddRowAndColumnDefinitionsAction Create(AddRowAndColumnDefinitionsTag tag, string file)
        {
            var result = new AddRowAndColumnDefinitionsAction(file)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
