﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXaml.EditorExtras.SymbolVisualizer
{
    /// <summary>
    /// Helper class for interspersing adornments into text.
    /// </summary>
    /// <remarks>
    /// To avoid an issue around intra-text adornment support and its interaction with text buffer changes,
    /// this tagger reacts to text and link tag changes with a delay. It waits to send out its own TagsChanged
    /// event until the WPF Dispatcher is running again and it takes care to report adornments
    /// that are consistent with the latest sent TagsChanged event by storing that particular snapshot
    /// and using it to query for the data tags.
    /// </remarks>
    /// <typeparam name="TData">The tag containing the data.</typeparam>
    /// <typeparam name="TAdornment">The type of the adornment.</typeparam>
    internal abstract class IntraTextAdornmentTagger<TData, TAdornment>
        : ITagger<IntraTextAdornmentTag>
        where TAdornment : UIElement
    {
#pragma warning disable SA1401 // Fields should be private - need access in inheritors
        protected readonly IWpfTextView view;
#pragma warning restore SA1401 // Fields should be private

        private readonly List<SnapshotSpan> invalidatedSpans = new List<SnapshotSpan>();

        private Dictionary<SnapshotSpan, TAdornment> adornmentCache = new Dictionary<SnapshotSpan, TAdornment>();

        protected IntraTextAdornmentTagger(IWpfTextView view)
        {
            this.view = view;
            this.Snapshot = view.TextBuffer.CurrentSnapshot;

            this.view.LayoutChanged += this.HandleLayoutChanged;
            this.view.TextBuffer.Changed += this.HandleBufferChanged;

            this.view.Closed += (s, e) =>
            {
                this.view.LayoutChanged -= this.HandleLayoutChanged;
                if (this.view.TextBuffer != null)
                {
                    this.view.TextBuffer.Changed -= this.HandleBufferChanged;
                }
            };
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        protected ITextSnapshot Snapshot { get; private set; }

        // Produces tags on the snapshot that the tag consumer asked for.
        public virtual IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans == null || spans.Count == 0)
            {
                yield break;
            }

            // Translate the request to the snapshot that this tagger is current with.
            ITextSnapshot requestedSnapshot = spans[0].Snapshot;

            var translatedSpans = new NormalizedSnapshotSpanCollection(spans.Select(span => span.TranslateTo(this.Snapshot, SpanTrackingMode.EdgeExclusive)));

            // Grab the adornments.
            foreach (var tagSpan in this.GetAdornmentTagsOnSnapshot(translatedSpans))
            {
                // Translate each adornment to the snapshot that the tagger was asked about.
                SnapshotSpan span = tagSpan.Span.TranslateTo(requestedSnapshot, SpanTrackingMode.EdgeExclusive);

                IntraTextAdornmentTag tag = new IntraTextAdornmentTag(tagSpan.Tag.Adornment, tagSpan.Tag.RemovalCallback, tagSpan.Tag.Affinity);
                yield return new TagSpan<IntraTextAdornmentTag>(span, tag);
            }

            yield break;
        }

        protected abstract TAdornment CreateAdornment(TData data, SnapshotSpan span);

        // Return True if the adornment was updated and should be kept. False to have the adornment removed from the view.
        protected abstract bool UpdateAdornment(TAdornment adornment, TData data);

        /// <summary>Get the adornments within the span collection</summary>
        /// <param name="spans">Spans to provide adornment data for. These spans do not necessarily correspond to text lines.</param>
        /// <remarks>
        /// If adornments need to be updated, call <see cref="RaiseTagsChanged"/> or <see cref="InvalidateSpans"/>.
        /// This will, indirectly, cause <see cref="GetAdornmentData"/> to be called.
        /// </remarks>
        /// <returns>
        /// A sequence of:
        ///  * adornment data for each adornment to be displayed
        ///  * the span of text that should be elided for that adornment (zero length spans are acceptable)
        ///  * and affinity of the adornment (this should be null if and only if the elided span has a length greater than zero).
        /// </returns>
        protected abstract IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, TData>> GetAdornmentData(NormalizedSnapshotSpanCollection spans);

        /// <summary>
        /// Causes intra-text adornments to be updated asynchronously.
        /// </summary>
        protected void InvalidateSpans(IList<SnapshotSpan> spans)
        {
            lock (this.invalidatedSpans)
            {
                bool wasEmpty = this.invalidatedSpans.Count == 0;
                this.invalidatedSpans.AddRange(spans);

                if (wasEmpty && this.invalidatedSpans.Count > 0)
                {
                    ThreadHelper.JoinableTaskFactory.Run(async () =>
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        this.AsyncUpdate();
                    });
                }
            }
        }

        /// <summary>
        /// Causes intra-text adornments to be updated synchronously.
        /// </summary>
        protected void RaiseTagsChanged(SnapshotSpan span)
        {
            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
        }

        private void HandleBufferChanged(object sender, TextContentChangedEventArgs args)
        {
            var editedSpans = args.Changes.Select(change => new SnapshotSpan(args.After, change.NewSpan)).ToList();
            this.InvalidateSpans(editedSpans);
        }

        private void AsyncUpdate()
        {
            // Store the snapshot that we're now current with and send an event
            // for the text that has changed.
            if (this.Snapshot != this.view.TextBuffer.CurrentSnapshot)
            {
                this.Snapshot = this.view.TextBuffer.CurrentSnapshot;

                Dictionary<SnapshotSpan, TAdornment> translatedAdornmentCache = new Dictionary<SnapshotSpan, TAdornment>();

                foreach (var keyValuePair in this.adornmentCache)
                {
                    var adjustedKey = keyValuePair.Key.TranslateTo(this.Snapshot, SpanTrackingMode.EdgeExclusive);

                    if (!translatedAdornmentCache.ContainsKey(adjustedKey))
                    {
                        translatedAdornmentCache.Add(adjustedKey, keyValuePair.Value);
                    }
                }

                this.adornmentCache = translatedAdornmentCache;
            }

            List<SnapshotSpan> translatedSpans;
            lock (this.invalidatedSpans)
            {
                translatedSpans = this.invalidatedSpans.Select(s => s.TranslateTo(this.Snapshot, SpanTrackingMode.EdgeInclusive)).ToList();
                this.invalidatedSpans.Clear();
            }

            if (translatedSpans.Count == 0)
            {
                return;
            }

            var start = translatedSpans.Select(span => span.Start).Min();
            var end = translatedSpans.Select(span => span.End).Max();

            this.RaiseTagsChanged(new SnapshotSpan(start, end));
        }

        private void HandleLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            SnapshotSpan visibleSpan = this.view.TextViewLines.FormattedSpan;

            // Filter out the adornments that are no longer visible.
            List<SnapshotSpan> toRemove = new List<SnapshotSpan>(
                from keyValuePair
                in this.adornmentCache
                where !keyValuePair.Key.TranslateTo(visibleSpan.Snapshot, SpanTrackingMode.EdgeExclusive).IntersectsWith(visibleSpan)
                select keyValuePair.Key);

            foreach (var span in toRemove)
            {
                this.adornmentCache.Remove(span);
            }
        }

        // Produces tags on the snapshot that this tagger is current with.
        private IEnumerable<TagSpan<IntraTextAdornmentTag>> GetAdornmentTagsOnSnapshot(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            ITextSnapshot snapshot = spans[0].Snapshot;

            System.Diagnostics.Debug.Assert(snapshot == this.Snapshot, "Snapshots out of sync.");

            // Since WPF UI objects have state (like mouse hover or animation) and are relatively expensive to create and lay out,
            // this code tries to reuse controls as much as possible.
            // The controls are stored in this.adornmentCache between the calls.

            // Mark which adornments fall inside the requested spans with Keep=false
            // so that they can be removed from the cache if they no longer correspond to data tags.
            HashSet<SnapshotSpan> toRemove = new HashSet<SnapshotSpan>();
            foreach (var ar in this.adornmentCache)
            {
                if (spans.IntersectsWith(new NormalizedSnapshotSpanCollection(ar.Key)))
                {
                    toRemove.Add(ar.Key);
                }
            }

            foreach (var spanDataPair in this.GetAdornmentData(spans).Distinct(new Comparer()))
            {
                // Look up the corresponding adornment or create one if it's new.
                SnapshotSpan snapshotSpan = spanDataPair.Item1;
                PositionAffinity? affinity = spanDataPair.Item2;
                TData adornmentData = spanDataPair.Item3;

                if (this.adornmentCache.TryGetValue(snapshotSpan, out TAdornment adornment))
                {
                    if (this.UpdateAdornment(adornment, adornmentData))
                    {
                        toRemove.Remove(snapshotSpan);
                    }
                }
                else
                {
                    adornment = this.CreateAdornment(adornmentData, snapshotSpan);

                    if (adornment == null)
                    {
                        continue;
                    }

                    // Get the adornment to measure itself. Its DesiredSize property is used to determine
                    // how much space to leave between text for this adornment.
                    // Note: If the size of the adornment changes, the line will be reformatted to accommodate it.
                    // Note: Some adornments may change size when added to the view's visual tree due to inherited
                    // dependency properties that affect layout. Such options can include SnapsToDevicePixels,
                    // UseLayoutRounding, TextRenderingMode, TextHintingMode, and TextFormattingMode. Making sure
                    // that these properties on the adornment match the view's values before calling Measure here
                    // can help avoid the size change and the resulting unnecessary re-format.
                    adornment.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                    this.adornmentCache.Add(snapshotSpan, adornment);
                }

                yield return new TagSpan<IntraTextAdornmentTag>(snapshotSpan, new IntraTextAdornmentTag(adornment, null, affinity));
            }

            foreach (var snapshotSpan in toRemove)
            {
                this.adornmentCache.Remove(snapshotSpan);
            }

            yield break;
        }

        private class Comparer : IEqualityComparer<Tuple<SnapshotSpan, PositionAffinity?, TData>>
        {
            public bool Equals(Tuple<SnapshotSpan, PositionAffinity?, TData> x, Tuple<SnapshotSpan, PositionAffinity?, TData> y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.Item1.Equals(y.Item1);
            }

            public int GetHashCode(Tuple<SnapshotSpan, PositionAffinity?, TData> obj)
            {
                return obj.Item1.GetHashCode();
            }
        }
    }
}
