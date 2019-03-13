// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class CheckboxContentAction : HardCodedStringAction
    {
        private CheckboxContentAction(string file, ITextView textView)
            : base(file, textView, Elements.CheckBox, Attributes.Content)
        {
        }

        public static CheckboxContentAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new CheckboxContentAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
