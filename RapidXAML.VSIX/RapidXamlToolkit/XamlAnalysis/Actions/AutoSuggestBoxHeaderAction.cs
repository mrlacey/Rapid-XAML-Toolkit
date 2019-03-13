// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AutoSuggestBoxHeaderAction : HardCodedStringAction
    {
        private AutoSuggestBoxHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.AutoSuggestBox, Attributes.Header)
        {
        }

        public static AutoSuggestBoxHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new AutoSuggestBoxHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
