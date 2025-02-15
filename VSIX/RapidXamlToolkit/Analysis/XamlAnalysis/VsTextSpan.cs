// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class VsTextSpan : ISpanAbstraction
    {
        private Span underlyingSpan;

        public VsTextSpan(int start, int length)
        {
            this.underlyingSpan = new Span(start, length);
        }

        public int Start => this.underlyingSpan.Start;

        public int Length => this.underlyingSpan.Length;
    }
}
