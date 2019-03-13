// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AppBarToggleButtonLabelAction : HardCodedStringAction
    {
        private AppBarToggleButtonLabelAction(string file, ITextView textView)
            : base(file, textView, Elements.AppBarToggleButton, Attributes.Label)
        {
        }

        public static AppBarToggleButtonLabelAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new AppBarToggleButtonLabelAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
