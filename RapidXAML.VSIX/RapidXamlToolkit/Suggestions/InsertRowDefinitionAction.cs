using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace RapidXamlToolkit.Suggestions
{
    public class InsertRowDefinitionAction : BaseSuggestedAction
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
}
