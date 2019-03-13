// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class PivotItemHeaderAction : HardCodedStringAction
    {
        private PivotItemHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.PivotItem, Attributes.Header)
        {
        }

        public static PivotItemHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new PivotItemHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
