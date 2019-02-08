using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Suggestions;
using RapidXamlToolkit.Tagging;

public class HardCodedStringTag : IRapidXamlViewTag
{
    public ActionTypes ActionType => ActionTypes.HardCodedString;

    public Span Span { get; set; }

    public int Line { get; set; }

    public int Column { get; set; }

    public ITextSnapshot Snapshot { get; set; }

    public ITagSpan<IErrorTag> AsErrorTag()
    {
        var span = new SnapshotSpan(this.Snapshot, this.Span);
        return new TagSpan<IErrorTag>(span, new ErrorTag(PredefinedErrorTypeNames.Warning, "HardCoded string message"));
    }

    public XamlWarning AsXamlWarning()
    {
        var span = new SnapshotSpan(this.Snapshot, this.Span);
        var result = new XamlWarning(span);

        return result;
    }
}
