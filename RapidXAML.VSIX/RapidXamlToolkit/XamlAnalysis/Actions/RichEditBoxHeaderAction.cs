// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class RichEditBoxHeaderAction : HardCodedStringAction
    {
        private RichEditBoxHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.RichEditBox, Attributes.Header)
        {
        }

        public static RichEditBoxHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new RichEditBoxHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
