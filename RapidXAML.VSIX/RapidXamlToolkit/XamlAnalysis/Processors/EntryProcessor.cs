using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EntryProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            // TODO: implement basic entry processor

            if (!XamlAnalysisHelpers.HasAttribute(Attributes.Keyboard))
            {
                // TODO: Create tag that indicates adding a Keyboard attribute
            }

            // TODO: implement advanced entry processor - suggested keyboard based on other info in the element (e.g. bound property name, header, etc.)
        }
    }
}
