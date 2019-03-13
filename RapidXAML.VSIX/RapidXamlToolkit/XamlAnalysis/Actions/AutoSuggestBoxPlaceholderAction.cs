// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AutoSuggestBoxPlaceholderAction : HardCodedStringAction
    {
        private AutoSuggestBoxPlaceholderAction(string file, ITextView textView)
            : base(file, textView, Elements.AutoSuggestBox, Attributes.PlaceholderText)
        {
        }

        public static AutoSuggestBoxPlaceholderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new AutoSuggestBoxPlaceholderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
