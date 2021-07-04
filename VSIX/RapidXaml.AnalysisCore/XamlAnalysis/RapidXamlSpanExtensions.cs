// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class RapidXamlSpanExtensions
    {
        public static RapidXamlSpan AddStartPos(this RapidXamlSpan source, int fileStartPos)
        {
            return new RapidXamlSpan(fileStartPos + source.Start, source.Length);
        }

        public static int End(this RapidXamlSpan source)
        {
            return source.Start + source.Length;
        }
    }
}
