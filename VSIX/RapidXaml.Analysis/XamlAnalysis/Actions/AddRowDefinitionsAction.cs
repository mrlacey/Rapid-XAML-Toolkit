// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddRowDefinitionsAction : InjectFixedXamlSuggestedAction
    {
        public AddRowDefinitionsAction(string file)
            : base(file)
        {
            this.InjectedXaml = @"<Grid.RowDefinitions>
    <RowDefinition Height=""Auto"" />
    <RowDefinition Height=""*"" />
</Grid.RowDefinitions>";

            this.UndoOperationName = StringRes.UI_UndoContextAddRowDefinitions;
            this.DisplayText = StringRes.UI_AddRowDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.TwoRows;

        public static AddRowDefinitionsAction Create(AddRowDefinitionsTag tag, string file)
        {
            var result = new AddRowDefinitionsAction(file)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
