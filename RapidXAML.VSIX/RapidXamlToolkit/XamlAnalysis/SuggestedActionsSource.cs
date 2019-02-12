// Copyright (c) Microsoft Corporation. All rights reserved.
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
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class SuggestedActionsSource : ISuggestedActionsSource, ISuggestedActionsSource2
    {
        private readonly ITextView _view;
        private string _file;
        private IViewTagAggregatorFactoryService _tagService;
        private readonly ISuggestedActionCategoryRegistryService _suggestedActionCategoryRegistry;

        public SuggestedActionsSource(IViewTagAggregatorFactoryService tagService, ISuggestedActionCategoryRegistryService suggestedActionCategoryRegistry, ITextView view, ITextBuffer textBuffer, string file)
        {
            this._tagService = tagService;
            this._suggestedActionCategoryRegistry = suggestedActionCategoryRegistry;
            this._view = view;
            this._file = file;

            this._view.LayoutChanged += this.OnViewLayoutChanged;

            RapidXamlDocumentCache.Add(this._file, textBuffer.CurrentSnapshot);
        }

        public event EventHandler<EventArgs> SuggestedActionsChanged
        {
            add { }
            remove { }
        }

        public Task<ISuggestedActionCategorySet> GetSuggestedActionCategoriesAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    if (this.GetTags(range).Any(t => t is RapidXamlErrorListTag))
                    {
                        return this._suggestedActionCategoryRegistry.CreateSuggestedActionCategorySet(
                            PredefinedSuggestedActionCategoryNames.Any);
                    }
                    else
                    {
                        return this._suggestedActionCategoryRegistry.CreateSuggestedActionCategorySet(
                            PredefinedSuggestedActionCategoryNames.Refactoring);
                    }
                },
                cancellationToken,
                TaskCreationOptions.None,
                TaskScheduler.Current);
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => { return this.GetTags(range).Any(); }, cancellationToken);
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var list = new List<SuggestedActionSet>();

            var rxTags = this.GetTags(range);

            foreach (var rapidXamlTag in rxTags)
            {
                if (rapidXamlTag.SuggestedAction == typeof(InsertRowDefinitionAction))
                {
                    list.AddRange(this.CreateActionSet(InsertRowDefinitionAction.Create((InsertRowDefinitionTag)rapidXamlTag, this._file, this._view)));
                }
                else if (rapidXamlTag.SuggestedAction == typeof(HardCodedStringAction))
                {
                    list.AddRange(this.CreateActionSet(HardCodedStringAction.Create((HardCodedStringTag)rapidXamlTag, this._file, this._view)));
                }
                else if (rapidXamlTag.SuggestedAction == typeof(OtherHardCodedStringAction))
                {
                    list.AddRange(this.CreateActionSet(OtherHardCodedStringAction.Create((OtherHardCodedStringTag)rapidXamlTag, this._file, this._view)));
                }
                else if (rapidXamlTag.SuggestedAction == typeof(AddRowAndColumnDefinitionsAction))
                {
                    list.AddRange(this.CreateActionSet(AddRowDefinitionsAction.Create((AddRowDefinitionsTag)rapidXamlTag)));
                }
                else if (rapidXamlTag.SuggestedAction == typeof(AddColumnDefinitionsAction))
                {
                    list.AddRange(this.CreateActionSet(AddColumnDefinitionsAction.Create((AddColumnDefinitionsTag)rapidXamlTag)));
                }
                else if (rapidXamlTag.SuggestedAction == typeof(AddRowAndColumnDefinitionsAction))
                {
                    list.AddRange(this.CreateActionSet(AddRowAndColumnDefinitionsAction.Create((AddRowAndColumnDefinitionsTag)rapidXamlTag)));
                }
            }

            return list;
        }

        private void OnViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // TODO: throttle this so doesn't fire until after a period of inactivity (1 second?)
            // Layout change can happen a lot, but only interested in if the text has changed
            if (e.OldSnapshot != e.NewSnapshot)
            {
                // TODO: SUPER OPTIMIZATION handle just the changed lines, rather than the whole document - would improve perf but might be very difficult for abstracted taggers
                RapidXamlDocumentCache.Update(this._file, e.NewViewState.EditSnapshot);
            }
        }

        private IEnumerable<IRapidXamlTag> GetTags(SnapshotSpan span)
        {
            return RapidXamlDocumentCache.AdornmentTags(this._file).Where(t => t.Span.IntersectsWith(span)).Select(t => t);
        }

        private IEnumerable<IMappingTagSpan<IRapidXamlTag>> GetErrorTags(ITextView view, SnapshotSpan span)
        {
            return this._tagService.CreateTagAggregator<IRapidXamlTag>(view).GetTags(span);
        }

        public IEnumerable<SuggestedActionSet> CreateActionSet(params BaseSuggestedAction[] actions)
        {
            var enabledActions = actions.Where(action => action.IsEnabled);
            return new[]
            {
                new SuggestedActionSet(
                    PredefinedSuggestedActionCategoryNames.Refactoring,
                    actions: enabledActions,
                    title: "Rapid XAML",
                    priority: SuggestedActionSetPriority.None),
            };
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // TODO: find out if we need this and what value to use if we do
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
