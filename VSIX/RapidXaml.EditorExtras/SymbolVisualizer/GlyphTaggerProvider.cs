// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace RapidXaml.EditorExtras.SymbolVisualizer
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("XAML")]
    [TagType(typeof(SymbolIconTag))]
    internal sealed class GlyphTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return buffer.Properties.GetOrCreateSingletonProperty(() => new GlyphTagger(buffer)) as ITagger<T>;
        }
    }
}
