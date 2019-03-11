// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ButtonContentAction : HardCodedStringAction
    {
        private ButtonContentAction(string file, ITextView textView)
            : base(file, textView, Elements.TextBlock, Attributes.Text)
        {
        }

        public static ButtonContentAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new ButtonContentAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
