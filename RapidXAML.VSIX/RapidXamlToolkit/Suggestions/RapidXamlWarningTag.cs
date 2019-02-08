using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.Suggestions
{
    public class RapidXamlWarningTag : ErrorTag
    {
        public RapidXamlWarningTag(string tooltip)
            : base(PredefinedErrorTypeNames.Warning, tooltip)
        {
        }
    }
}
