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
        public AddColumnDefinitionsAction()
        {
            this.InjectedXaml = @"<Grid.ColumnDefinitions>
    <ColumnDefinition Width=""*"" />
    <ColumnDefinition Width=""*"" />
</Grid.ColumnDefinitions>";

            this.UndoOperationName = StringRes.Info_UndoContextAddColumnDefinitons;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.TwoColumns;

        public override string DisplayText { get; } = "Add ColumnDefinitions";  // TODO: localize

        public static AddColumnDefinitionsAction Create(AddColumnDefinitionsTag tag)
        {
            var result = new AddColumnDefinitionsAction
            {
                Tag = tag,
            };

            return result;
        }
    }
}
