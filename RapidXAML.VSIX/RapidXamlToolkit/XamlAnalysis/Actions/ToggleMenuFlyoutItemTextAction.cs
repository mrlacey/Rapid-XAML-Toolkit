// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ToggleMenuFlyoutItemTextAction : HardCodedStringAction
    {
        private ToggleMenuFlyoutItemTextAction(string file, ITextView textView)
            : base(file, textView, Elements.ToggleMenuFlyoutItem, Attributes.Text)
        {
        }

        public static ToggleMenuFlyoutItemTextAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new ToggleMenuFlyoutItemTextAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
