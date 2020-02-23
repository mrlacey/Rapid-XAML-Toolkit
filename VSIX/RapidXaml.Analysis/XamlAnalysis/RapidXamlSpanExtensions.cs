// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class RapidXamlSpanExtensions
    {
        public static Span ToSpanPlusStartPos(this RapidXamlSpan source, int fileStartPos)
        {
            return new Span(fileStartPos + source.Start, source.Length);
        }
    }
}
