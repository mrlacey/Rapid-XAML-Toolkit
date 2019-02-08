using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
    public class HardCodedStringTag : RapidXamlViewTag
    {
        public HardCodedStringTag()
        {
            this.ActionType = ActionTypes.HardCodedString;
            this.ToolTip = "HardCoded string message";
        }
    }
}
