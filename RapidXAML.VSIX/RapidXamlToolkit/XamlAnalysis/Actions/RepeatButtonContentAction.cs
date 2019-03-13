// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class RepeatButtonContentAction : HardCodedStringAction
    {
        private RepeatButtonContentAction(string file, ITextView textView)
            : base(file, textView, Elements.RepeatButton, Attributes.Content)
        {
        }

        public static RepeatButtonContentAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new RepeatButtonContentAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
