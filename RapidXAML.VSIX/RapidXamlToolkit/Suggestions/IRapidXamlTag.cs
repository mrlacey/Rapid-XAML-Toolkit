using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.Suggestions
{
    public interface IRapidXamlTag : ITag
    {
        ActionTypes ActionType { get; }

        Span Span { get; set; }
    }
}
