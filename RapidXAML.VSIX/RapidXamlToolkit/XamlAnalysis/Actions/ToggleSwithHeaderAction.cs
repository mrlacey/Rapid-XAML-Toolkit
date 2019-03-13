// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ToggleSwithHeaderAction : HardCodedStringAction
    {
        private ToggleSwithHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.ToggleSwitch, Attributes.Header)
        {
        }

        public static ToggleSwithHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new ToggleSwithHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
