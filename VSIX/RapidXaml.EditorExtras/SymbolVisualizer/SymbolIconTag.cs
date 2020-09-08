// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXaml.EditorExtras.SymbolVisualizer
{
    internal class SymbolIconTag : ITag
    {
        public SymbolIconTag(string symbolName, SymbolType symbolType, string fontFamily = "")
        {
            this.SymbolName = symbolName;
            this.SymbolType = symbolType;
            this.FontFamily = fontFamily;
        }

        public string SymbolName { get; }

        public SymbolType SymbolType { get; }

        public string FontFamily { get; }
    }
}
