// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class LineInsertionTag : RapidXamlOptionalTag
    {
        protected LineInsertionTag(Span span, ITextSnapshot snapshot)
            : base(span, snapshot)
        {
        }

        public int InsertLine { get; set; }
    }
}
