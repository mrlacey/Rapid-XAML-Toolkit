// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddRowDefinitionsAction : InjectFixedXamlSuggestedAction
    {
        public AddRowDefinitionsAction()
        {
            this.InjectedXaml = @"<Grid.RowDefinitions>
    <RowDefinition Height=""Auto"" />
    <RowDefinition Height=""*"" />
</Grid.RowDefinitions>";

            this.UndoOperationName = StringRes.Info_UndoContextAddRowDefinitions;
            this.DisplayText = StringRes.UI_AddRowDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.TwoRows;

        public static AddRowDefinitionsAction Create(AddRowDefinitionsTag tag)
        {
            var result = new AddRowDefinitionsAction
            {
                Tag = tag,
            };

            return result;
        }
    }
}
