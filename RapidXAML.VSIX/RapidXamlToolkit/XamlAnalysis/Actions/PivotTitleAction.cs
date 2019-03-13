// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class PivotTitleAction : HardCodedStringAction
    {
        private PivotTitleAction(string file, ITextView textView)
            : base(file, textView, Elements.Pivot, Attributes.Title)
        {
        }

        public static PivotTitleAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new PivotTitleAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
