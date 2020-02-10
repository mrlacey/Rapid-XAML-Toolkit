// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class InsertionTag : RapidXamlDiscreteTag
    {
        protected InsertionTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, logger)
        {
        }

        public bool GridNeedsExpanding { get; set; }

        public int InsertPosition { get; set; }

        // Whitespace to put before each line when added to the document (but not in preview).
        public string LeftPad { get; set; }
    }
}
