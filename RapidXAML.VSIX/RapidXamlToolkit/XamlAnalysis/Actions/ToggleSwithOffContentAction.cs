// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ToggleSwithOffContentAction : HardCodedStringAction
    {
        private ToggleSwithOffContentAction(string file, ITextView textView)
            : base(file, textView, Elements.ToggleSwitch, Attributes.OffContent)
        {
        }

        public static ToggleSwithOffContentAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new ToggleSwithOffContentAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
