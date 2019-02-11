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
    public class SuggestedActionsSource : ISuggestedActionsSource
    {
        private readonly ITextView _view;
        private string _file;
        private IViewTagAggregatorFactoryService _tagService;

        public SuggestedActionsSource(IViewTagAggregatorFactoryService tagService, ITextView view, ITextBuffer textBuffer, string file)
        {
            _tagService = tagService;
            _view = view;
            _file = file;

            _view.LayoutChanged += this.OnViewLayoutChanged;

            RapidXamlDocumentCache.Add(_file, textBuffer.CurrentSnapshot);
        }

        private void OnViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // TODO: throttle this so doesn't fire until after a period of inactivity (1 second?)
            // Layout change can happen a lot, but only interested in if the text has changed
            if (e.OldSnapshot != e.NewSnapshot)
            {
                // TODO: SUPER OPTIMIZATION handle just the changed lines, rather than the whole document - would improve perf but might be very difficult for abstracted taggers
                RapidXamlDocumentCache.Update(_file, e.NewViewState.EditSnapshot);
            }
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => this.GetTags(range).Any(), cancellationToken);
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            /*
            var span = new SnapshotSpan(_view.Selection.Start.Position, _view.Selection.End.Position);
            var startLine = span.Start.GetContainingLine().Extent;
            var endLine = span.End.GetContainingLine().Extent;

            var selectionStart = _view.Selection.Start.Position.Position;
            var selectionEnd = _view.Selection.End.Position.Position;
            var SelectedSpan = new SnapshotSpan(span.Snapshot, selectionStart, selectionEnd - selectionStart);
            */

            var list = new List<SuggestedActionSet>();

            var rxTags = this.GetTags(range);

            foreach (var rapidXamlTag in rxTags)
            {
                if (rapidXamlTag.ActionType == ActionTypes.InsertRowDefinition)
                {
                    list.AddRange(CreateActionSet(InsertRowDefinitionAction.Create((InsertRowDefinitionTag)rapidXamlTag, _file, _view)));
                }
                else if (rapidXamlTag.ActionType == ActionTypes.HardCodedString)
                {
                    list.AddRange(CreateActionSet(HardCodedStringAction.Create((HardCodedStringTag)rapidXamlTag, _file, _view)));
                }
                else if (rapidXamlTag.ActionType == ActionTypes.AddRowDefinitions)
                {
                    list.AddRange(CreateActionSet(AddRowDefinitionsAction.Create((AddRowDefinitionsTag)rapidXamlTag)));
                }
                else if (rapidXamlTag.ActionType == ActionTypes.AddColumnDefinitions)
                {
                    list.AddRange(CreateActionSet(AddColumnDefinitionsAction.Create((AddColumnDefinitionsTag)rapidXamlTag)));
                }
                else if (rapidXamlTag.ActionType == ActionTypes.AddRowAndColumnDefinitions)
                {
                    list.AddRange(CreateActionSet(AddRowAndColumnDefinitionsAction.Create((AddRowAndColumnDefinitionsTag)rapidXamlTag)));
                }
            }

            return list;
        }

        private IEnumerable<IRapidXamlTag> GetTags(SnapshotSpan span)
        {
            return RapidXamlDocumentCache.AdornmentTags(_file).Where(t => t.Span.IntersectsWith(span)).Select(t => t);
        }

        private IEnumerable<IMappingTagSpan<IRapidXamlTag>> GetErrorTags(ITextView view, SnapshotSpan span)
        {
            return _tagService.CreateTagAggregator<IRapidXamlTag>(view).GetTags(span);
        }

        public IEnumerable<SuggestedActionSet> CreateActionSet(params BaseSuggestedAction[] actions)
        {
            var enabledActions = actions.Where(action => action.IsEnabled);
            return new[] { new SuggestedActionSet(enabledActions) };
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample provider and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }

        public event EventHandler<EventArgs> SuggestedActionsChanged
        {
            add { }
            remove { }
        }
    }
}
