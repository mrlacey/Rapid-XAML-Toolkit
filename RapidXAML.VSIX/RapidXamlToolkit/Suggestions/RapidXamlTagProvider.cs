using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Tagging;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Suggestions
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("XAML")]
    public class RapidXamlTagProvider : ITaggerProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            ITextDocument document;

            if (!TextDocumentFactoryService.TryGetTextDocument(buffer, out document))
            {
                return null;
            }

            return buffer.Properties.GetOrCreateSingletonProperty(() => new RapidXamlTagger(buffer, document.FilePath)) as ITagger<T>;
        }
    }

    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Rapid XAML Suggested Actions")]
    [ContentType("XAML")]
    class SuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
    {
        ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService { get; set; }

        [ImportingConstructor]
        public SuggestedActionsSourceProvider(IViewTagAggregatorFactoryService viewTagAggregatorFactoryService, ITextDocumentFactoryService textDocumentFactoryService)
        {
            ViewTagAggregatorFactoryService = viewTagAggregatorFactoryService;
            TextDocumentFactoryService = textDocumentFactoryService;
        }

        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            ITextDocument document;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextBuffer, out document))
            {
                return textView.Properties.GetOrCreateSingletonProperty(() =>
                    new SuggestedActionsSource(ViewTagAggregatorFactoryService, textView, textBuffer, document.FilePath));
            }

            return null;
        }
    }

    class SuggestedActionsSource : ISuggestedActionsSource
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
            // Layout change can happen a lot, but only interested in if the text has changed
            if (e.OldSnapshot != e.NewSnapshot)
            {
                // TODO: handle just the changed lines, rather than the whole document - would improve perf but might be very difficult for abstracted taggers
                RapidXamlDocumentCache.Update(_file, e.NewViewState.EditSnapshot);
            }
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                return GetTags(range).Any();

                //    return true; // If range intersects with any known issues

                // return !_view.Selection.IsEmpty;
            });
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var span = new SnapshotSpan(_view.Selection.Start.Position, _view.Selection.End.Position);
            var startLine = span.Start.GetContainingLine().Extent;
            var endLine = span.End.GetContainingLine().Extent;

            var selectionStart = _view.Selection.Start.Position.Position;
            var selectionEnd = _view.Selection.End.Position.Position;
            var SelectedSpan = new SnapshotSpan(span.Snapshot, selectionStart, selectionEnd - selectionStart);

            var list = new List<SuggestedActionSet>();


            var rxTags = GetTags(range);

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
            }

            return list;
        }

        private IEnumerable<IRapidXamlTag> GetTags(SnapshotSpan span)
        {
            return RapidXamlDocumentCache.Tags(_file).Where(t => t.Span.IntersectsWith(span)).Select(t => t);
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

    public enum ActionTypes
    {
        InsertRowDefinition,
        HardCodedString,
    }

    public interface IRapidXamlViewTag : IRapidXamlTag
    {
        int Line { get; set; }

        int Column { get; set; }

        // TODO: move the following to an abstract base class?
        ITagSpan<IErrorTag> AsErrorTag();

        XamlWarning AsXamlWarning();

    }

    public interface IRapidXamlTag : ITag
    {
        ActionTypes ActionType { get; }

        Span Span { get; set; }
    }

    public abstract class BaseSuggestedAction : ISuggestedAction
    {
        public abstract string DisplayText { get; }

        public virtual bool IsEnabled { get; } = true;

        public virtual bool HasActionSets
        {
            get { return false; }
        }

        public virtual bool HasPreview
        {
            get { return false; }
        }

        public string IconAutomationText
        {
            get { return null; }
        }

        public virtual ImageMoniker IconMoniker
        {
            get { return default(ImageMoniker); }
        }

        public string InputGestureText
        {
            get { return null; }
        }

        public virtual void Dispose()
        {
            // nothing to dispose
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            try
            {
                ProjectHelpers.DTE.UndoContext.Open(DisplayText);
                Execute(cancellationToken);
            }
            finally
            {
                ProjectHelpers.DTE.UndoContext.Close();
            }
        }

        public abstract void Execute(CancellationToken cancellationToken);

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }

        protected static IEnumerable<ITextSnapshotLine> GetSelectedLines(SnapshotSpan span, out SnapshotSpan wholeSpan)
        {
            var startLine = span.Start.GetContainingLine();
            var endLine = span.End.GetContainingLine();

            wholeSpan = new SnapshotSpan(startLine.Start, endLine.End);
            return span.Snapshot.Lines.Where(l => l.LineNumber >= startLine.LineNumber && l.LineNumber <= endLine.LineNumber);
        }
    }

    public static class ProjectHelpers
    {
        static ProjectHelpers()
        {
            DTE = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public static DTE2 DTE { get; }
    }

    class InsertRowDefinitionAction : BaseSuggestedAction
    {
        private string file;
        private string linkUrl;
        private ITextView view;
        public InsertRowDefinitionTag tag;

        public override string DisplayText
        {
            get { return $"Insert new definition for row {tag.RowId}"; }
        }

        public override ImageMoniker IconMoniker
        {
            get { return KnownMonikers.InsertClause; }
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            // TODO: Do insertion
        }

        // TODO call this after having made the change to force reevaluation of actions - move to base?
        private void RaiseBufferChange()
        {
            // Adding and deleting a char in order to force taggers re-evaluation
            string text = " ";
            view.TextBuffer.Insert(0, text);
            view.TextBuffer.Delete(new Span(0, text.Length));
        }

        public static InsertRowDefinitionAction Create(InsertRowDefinitionTag tag, string file, ITextView view)
        {
            // var errorTag = errorTags
            //     .Select(m => m.Tag as InsertRowDefinitionTag)
            //     .Where(tag => tag != null)
            //     .FirstOrDefault();
            //
            // if (errorTag == null)
            // {
            //     return null;
            // }

            var result = new InsertRowDefinitionAction
            {
                // linkUrl = errorTag.Url,
                tag = tag,
                file = file,
                view = view,
            };

            return result;
        }
    }

    public class HardCodedStringAction : BaseSuggestedAction
    {
        private string file;
        private ITextView view;
        public HardCodedStringTag tag;

        public override ImageMoniker IconMoniker => KnownMonikers.GenerateResource;

        public override string DisplayText
        {
            get { return "Move hard coded string to resource file."; }
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static HardCodedStringAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HardCodedStringAction
            {
                // linkUrl = errorTag.Url,
                tag = tag,
                file = file,
                view = view,
            };

            return result;
        }
    }
}
