// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class InsertionTag : RapidXamlDiscreteTag
    {
        protected InsertionTag(Span span, ITextSnapshot snapshot, string fileName)
            : base(span, snapshot, fileName)
        {
        }

        public bool GridNeedsExpanding { get; set; }

        public int InsertPosition { get; set; }

        // Whitespace to put before each line when added to the document (but not in preview).
        public string LeftPad { get; set; }
    }
}
