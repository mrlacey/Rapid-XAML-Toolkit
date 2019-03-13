// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class HyperlinkButtonContentAction : HardCodedStringAction
    {
        private HyperlinkButtonContentAction(string file, ITextView textView)
            : base(file, textView, Elements.HyperlinkButton, Attributes.Content)
        {
        }

        public static HyperlinkButtonContentAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HyperlinkButtonContentAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
