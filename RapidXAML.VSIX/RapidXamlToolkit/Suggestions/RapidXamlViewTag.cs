using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
    public abstract class RapidXamlViewTag : IRapidXamlViewTag
    {
        public ActionTypes ActionType { get; protected set; }

        public Span Span { get; set; }

        public string ToolTip { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public ITagSpan<IErrorTag> AsErrorTag()
        {
            var span = new SnapshotSpan(this.Snapshot, this.Span);
            return new TagSpan<IErrorTag>(span, new RapidXamlWarningTag(this.ToolTip));
        }

        public XamlWarning AsXamlWarning()
        {
            var span = new SnapshotSpan(this.Snapshot, this.Span);
            var result = new XamlWarning(span);

            return result;
        }
    }
}
