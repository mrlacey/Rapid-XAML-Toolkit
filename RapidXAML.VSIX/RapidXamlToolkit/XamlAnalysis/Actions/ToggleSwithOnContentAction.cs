// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ToggleSwithOnContentAction : HardCodedStringAction
    {
        private ToggleSwithOnContentAction(string file, ITextView textView)
            : base(file, textView, Elements.ToggleSwitch, Attributes.OnContent)
        {
        }

        public static ToggleSwithOnContentAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new ToggleSwithOnContentAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
