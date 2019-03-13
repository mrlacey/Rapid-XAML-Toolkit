// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class MenuFlyoutItemTextAction : HardCodedStringAction
    {
        private MenuFlyoutItemTextAction(string file, ITextView textView)
            : base(file, textView, Elements.MenuFlyoutItem, Attributes.Text)
        {
        }

        public static MenuFlyoutItemTextAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new MenuFlyoutItemTextAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
