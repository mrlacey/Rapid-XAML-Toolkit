// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddColumnDefinitionsAction : InjectFixedXamlSuggestedAction
    {
        public AddColumnDefinitionsAction(string file)
            : base(file)
        {
            this.InjectedXaml = @"<Grid.ColumnDefinitions>
    <ColumnDefinition Width=""*"" />
    <ColumnDefinition Width=""*"" />
</Grid.ColumnDefinitions>";

            this.UndoOperationName = StringRes.Info_UndoContextAddColumnDefinitions;
            this.DisplayText = StringRes.UI_AddColumnDefinitions;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.TwoColumns;

        public static AddColumnDefinitionsAction Create(AddColumnDefinitionsTag tag, string file)
        {
            var result = new AddColumnDefinitionsAction(file)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
