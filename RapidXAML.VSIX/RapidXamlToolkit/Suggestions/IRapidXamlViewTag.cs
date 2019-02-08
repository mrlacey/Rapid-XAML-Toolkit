using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Suggestions;
using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Tagging
{
    public interface IRapidXamlViewTag : IRapidXamlTag
    {
        string ToolTip { get; set; }

        int Line { get; set; }

        int Column { get; set; }

        ITextSnapshot Snapshot { get; set; }

        ITagSpan<IErrorTag> AsErrorTag();

        XamlWarning AsXamlWarning();
    }
}
