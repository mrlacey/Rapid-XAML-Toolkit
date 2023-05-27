// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Configuration;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class SuggestedActionsSource : ISuggestedActionsSource, ISuggestedActionsSource2
    {
        private readonly RxtSettings config = new();
        private readonly ITextView view;
        private readonly ISuggestedActionCategoryRegistryService suggestedActionCategoryRegistry;
        private readonly string file;
        private readonly IViewTagAggregatorFactoryService tagService;

        public SuggestedActionsSource(IViewTagAggregatorFactoryService tagService, ISuggestedActionCategoryRegistryService suggestedActionCategoryRegistry, ITextView view, ITextBuffer textBuffer, string file)
        {
            this.tagService = tagService;
            this.suggestedActionCategoryRegistry = suggestedActionCategoryRegistry;
            this.view = view;
            this.file = file;

            RapidXamlDocumentCache.Add(this.file, textBuffer.CurrentSnapshot);
        }

        public event EventHandler<EventArgs> SuggestedActionsChanged;

        public void Refresh()
        {
            this.SuggestedActionsChanged.Invoke(this, new EventArgs());
        }

        public Task<ISuggestedActionCategorySet> GetSuggestedActionCategoriesAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    // Setting the only category to be "REFACTORING" causes the screwdriver icon to be shown, otherwise get the light bulb.
                    return this.suggestedActionCategoryRegistry.CreateSuggestedActionCategorySet(
                        this.GetTags(range).Any(t => t is RapidXamlDisplayedTag) ? PredefinedSuggestedActionCategoryNames.Any
                                                                                 : PredefinedSuggestedActionCategoryNames.Refactoring);
                },
                cancellationToken,
                TaskCreationOptions.None,
                TaskScheduler.Current);
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => this.GetTags(range).Any(), cancellationToken, TaskCreationOptions.None, TaskScheduler.Current);
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var list = new List<SuggestedActionSet>();

            try
            {
                var rxTags = this.GetTags(range);

                foreach (var rxTag in rxTags)
                {
                    if (rxTag is CustomAnalysisTag cat)
                    {
                        list.AddRange(this.CreateActionSet(rxTag, CustomAnalysisAction.Create(cat, this.file)));
                    }
                    else
                    {
                        switch (rxTag.GetType().Name)
                        {
                            case nameof(HardCodedStringTag):
                                list.AddRange(this.CreateActionSet(rxTag, new HardCodedStringAction(this.file, this.view, (HardCodedStringTag)rxTag)));
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                RapidXamlPackage.Logger?.RecordException(e);
            }

            return list;
        }

        public IEnumerable<SuggestedActionSet> CreateActionSet(IRapidXamlTag tag, params BaseSuggestedAction[] actions)
        {
            var result = new List<SuggestedActionSet>()
            {
                new SuggestedActionSet(
                    PredefinedSuggestedActionCategoryNames.Refactoring,
                    actions: actions,
                    title: StringRes.UI_SuggestedActionSetTitle,
                    priority: SuggestedActionSetPriority.None,
                    applicableToSpan: new Span(tag.Span.Start, tag.Span.Length)),
            };

            if (tag is RapidXamlDisplayedTag rxdt)
            {
                foreach (var action in actions)
                {
                    // Don't show a suppression action if there is no ErrorCode
                    if (!string.IsNullOrWhiteSpace(rxdt.ErrorCode))
                    {
                        result.Add(new SuggestedActionSet(
                            PredefinedSuggestedActionCategoryNames.Any,
                            actions: new[] { SuppressWarningAction.Create(rxdt, action.File, this) },
                            title: StringRes.UI_SuggestedActionSetTitle,
                            priority: SuggestedActionSetPriority.None,
                            applicableToSpan: new Span(tag.Span.Start, tag.Span.Length)));
                    }
                }
            }

            return result;
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = this.config.LightBulbTelemetryGuid;
            return false;
        }

        private void OnViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // It would be "nice" to only reparse the changed lines in large documents but would need to keep track or any processors that work on the encapsulated changes.
            // Caching processors for partial re-parsing would be complicated. Considering this a low priority optimization.
            if (e.OldSnapshot != e.NewSnapshot)
            {
                RapidXamlDocumentCache.Update(this.file, e.NewViewState.EditSnapshot);
            }
        }

        private IEnumerable<IRapidXamlTag> GetTags(SnapshotSpan span)
        {
            return RapidXamlDocumentCache.AdornmentTags(this.file).Where(t => span.IntersectsWith(new Span(t.Span.Start, t.Span.Length))).Select(t => t);
        }

        private IEnumerable<IMappingTagSpan<ITag>> GetErrorTags(ITextView textView, SnapshotSpan span)
        {
            return this.tagService.CreateTagAggregator<ITag>(textView).GetTags(span);
        }
    }
}
