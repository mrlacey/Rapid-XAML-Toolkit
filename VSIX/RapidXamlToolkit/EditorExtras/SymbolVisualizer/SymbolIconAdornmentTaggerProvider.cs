// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using RapidXamlToolkit;

namespace RapidXaml.EditorExtras.SymbolVisualizer
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(KnownContentTypes.Xaml)]
    [ContentType("projection")]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class SymbolIconAdornmentTaggerProvider : IViewTaggerProvider
    {
#pragma warning disable 649 // "field never assigned to" -- field is set by MEF.
        [Import]
#pragma warning disable SA1401 // Fields should be private
        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore 649

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer)
            where T : ITag
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer != textView.TextBuffer)
            {
                return null;
            }

            return SymbolIconAdornmentTagger.GetTagger(
                (IWpfTextView)textView,
                new Lazy<ITagAggregator<SymbolIconTag>>(
                    () => this.BufferTagAggregatorFactoryService.CreateTagAggregator<SymbolIconTag>(textView.TextBuffer)))
                as ITagger<T>;
        }
    }
}
