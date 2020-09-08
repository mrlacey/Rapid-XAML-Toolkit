// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;

namespace RapidXaml.EditorExtras.SymbolVisualizer
{
    internal sealed class SymbolIconTagger : SymbolIconRegexTagger
    {
        internal SymbolIconTagger(ITextBuffer buffer)
            : base(buffer, @"(Symbol="")([a-z2]{2,})("")")
        {
        }

        protected override SymbolIconTag TryCreateTagForMatch(Match match, int lineStart, int spanStart, string snapshotText)
        {
            return this.TryCreateSymbolIconTagForMatch(match, lineStart, spanStart, snapshotText, SymbolType.Symbol, "SymbolIcon");
        }
    }
}
