using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Suggestions
{
    public interface IRapidXamlViewTag : IRapidXamlTag
    {
        int Line { get; set; }

        int Column { get; set; }

        // TODO: move the following to an abstract base class?
        ITagSpan<IErrorTag> AsErrorTag();

        XamlWarning AsXamlWarning();

    }
}
