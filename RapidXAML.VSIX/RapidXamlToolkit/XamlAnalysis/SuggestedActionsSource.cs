// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Configuration;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class SuggestedActionsSource : ISuggestedActionsSource, ISuggestedActionsSource2
    {
        private readonly RxtSettings config = new RxtSettings();
        private readonly ITextView view;
        private readonly ISuggestedActionCategoryRegistryService suggestedActionCategoryRegistry;
        private string file;
        private IViewTagAggregatorFactoryService tagService;

        public SuggestedActionsSource(IViewTagAggregatorFactoryService tagService, ISuggestedActionCategoryRegistryService suggestedActionCategoryRegistry, ITextView view, ITextBuffer textBuffer, string file)
        {
            this.tagService = tagService;
            this.suggestedActionCategoryRegistry = suggestedActionCategoryRegistry;
            this.view = view;
            this.file = file;

            // Don't want every change event as that is a lot during editing. Wait for a second of inactivity before reparsing.
            this.WhenViewLayoutChanged.Throttle(TimeSpan.FromSeconds(1)).Subscribe(e => this.OnViewLayoutChanged(this, e));

            RapidXamlDocumentCache.Add(this.file, textBuffer.CurrentSnapshot);
        }

        // TODO: investigate if/when should be calling this
        public event EventHandler<EventArgs> SuggestedActionsChanged;

        // Observable event wrapper
        public IObservable<TextViewLayoutChangedEventArgs> WhenViewLayoutChanged
        {
            get
            {
                return Observable
                    .FromEventPattern<EventHandler<TextViewLayoutChangedEventArgs>, TextViewLayoutChangedEventArgs>(
                        h => this.view.LayoutChanged += h,
                        h => this.view.LayoutChanged -= h)
                    .Select(x => x.EventArgs);
            }
        }

        public Task<ISuggestedActionCategorySet> GetSuggestedActionCategoriesAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    // Setting the only category to be "REFACTORING" causes the screwdriver icon to be shown, otherwise get the light bulb.
                    return this.suggestedActionCategoryRegistry.CreateSuggestedActionCategorySet(
                        this.GetTags(range).Any(t => t is RapidXamlErrorListTag) ? PredefinedSuggestedActionCategoryNames.Any
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
                    switch (rxTag.SuggestedAction.Name)
                    {
                        case nameof(HardCodedStringAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, new HardCodedStringAction(this.file, this.view, (HardCodedStringTag)rxTag)));
                            break;
                        case nameof(InsertRowDefinitionAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, InsertRowDefinitionAction.Create((InsertRowDefinitionTag)rxTag, this.file, this.view)));
                            break;
                        case nameof(AddRowDefinitionsAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, AddRowDefinitionsAction.Create((AddRowDefinitionsTag)rxTag, this.file)));
                            break;
                        case nameof(AddColumnDefinitionsAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, AddColumnDefinitionsAction.Create((AddColumnDefinitionsTag)rxTag, this.file)));
                            break;
                        case nameof(AddRowAndColumnDefinitionsAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, AddRowAndColumnDefinitionsAction.Create((AddRowAndColumnDefinitionsTag)rxTag, this.file)));
                            break;
                        case nameof(AddMissingRowDefinitionsAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, AddMissingRowDefinitionsAction.Create((MissingRowDefinitionTag)rxTag, this.file)));
                            break;
                        case nameof(AddMissingColumnDefinitionsAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, AddMissingColumnDefinitionsAction.Create((MissingColumnDefinitionTag)rxTag, this.file)));
                            break;
                        case nameof(AddEntryKeyboardAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, AddEntryKeyboardAction.Create((AddEntryKeyboardTag)rxTag, this.file)));
                            break;
                        case nameof(AddTextBoxInputScopeAction):
                            list.AddRange(this.CreateActionSet(rxTag.Span, AddTextBoxInputScopeAction.Create((AddTextBoxInputScopeTag)rxTag, this.file)));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                RapidXamlPackage.Logger?.RecordException(e);
            }

            return list;
        }

        public IEnumerable<SuggestedActionSet> CreateActionSet(Span span, params BaseSuggestedAction[] actions)
        {
            var enabledActions = actions.Where(action => action.IsEnabled);
            return new[]
            {
                new SuggestedActionSet(
                    PredefinedSuggestedActionCategoryNames.Refactoring,
                    actions: enabledActions,
                    title: "Rapid XAML",
                    priority: SuggestedActionSetPriority.None,
                    applicableToSpan: span),
            };
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
            return RapidXamlDocumentCache.AdornmentTags(this.file).Where(t => t.Span.IntersectsWith(span)).Select(t => t);
        }

        private IEnumerable<IMappingTagSpan<IRapidXamlTag>> GetErrorTags(ITextView textView, SnapshotSpan span)
        {
            return this.tagService.CreateTagAggregator<IRapidXamlTag>(textView).GetTags(span);
        }
    }
}
