using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Suggestions;
using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Tagging
{
    public interface IRapidXamlViewTag : IRapidXamlTag
    {
        string ToolTip { get; set; }

        string Message { get; set; }

        int Line { get; set; }

        int Column { get; set; }

        ITextSnapshot Snapshot { get; set; }

        ITagSpan<IErrorTag> AsErrorTag();

        ErrorRow AsErrorRow();
    }
}
