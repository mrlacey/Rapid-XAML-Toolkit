// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXaml.EditorExtras.SymbolVisualizer
{
    internal sealed class SymbolIconAdornmentTagger
        : IntraTextAdornmentTagger<SymbolIconTag, SymbolIconAdornment>
    {
        private readonly ITagAggregator<SymbolIconTag> tagger;

        private SymbolIconAdornmentTagger(IWpfTextView view, ITagAggregator<SymbolIconTag> tagger)
            : base(view)
        {
            this.tagger = tagger;
        }

        public void Dispose()
        {
            this.tagger.Dispose();

            this.view.Properties.RemoveProperty(typeof(SymbolIconAdornmentTagger));
        }

        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<SymbolIconTag>> tagger)
        {
            return view.Properties.GetOrCreateSingletonProperty(
                () => new SymbolIconAdornmentTagger(view, tagger.Value));
        }

        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, SymbolIconTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            if (RapidXamlEditorExtrasPackage.Options?.ShowSymbolIcons != true)
            {
                yield break;
            }

            ITextSnapshot snapshot = spans[0].Snapshot;

            var clTags = this.tagger.GetTags(spans);

            foreach (IMappingTagSpan<SymbolIconTag> dataTagSpan in clTags)
            {
                NormalizedSnapshotSpanCollection linkTagSpans = dataTagSpan.Span.GetSpans(snapshot);

                // Ignore data tags that are split by projection.
                // This is theoretically possible but unlikely in current scenarios.
                if (linkTagSpans.Count != 1)
                {
                    continue;
                }

                SnapshotSpan adornmentSpan = new SnapshotSpan(linkTagSpans[0].Start, 0);

                yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag);
            }
        }

        protected override SymbolIconAdornment CreateAdornment(SymbolIconTag dataTag, SnapshotSpan span)
        {
            return new SymbolIconAdornment(dataTag);
        }

        protected override bool UpdateAdornment(SymbolIconAdornment adornment, SymbolIconTag dataTag)
        {
            adornment.Update(dataTag);
            return true;
        }
    }
}
