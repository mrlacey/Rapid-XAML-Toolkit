// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ComboBoxHeaderAction : HardCodedStringAction
    {
        private ComboBoxHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.ComboBox, Attributes.Header)
        {
        }

        public static ComboBoxHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new ComboBoxHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
