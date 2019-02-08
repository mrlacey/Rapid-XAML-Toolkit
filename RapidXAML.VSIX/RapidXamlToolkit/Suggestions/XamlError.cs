using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.Tagging
{
    public class XamlError
    {
        public string Project { get; set; }

        public string File { get; set; }

        public string Message { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public string ErrorCode { get; set; }

        public Span Span { get; set; }

        public bool Fatal { get; set; }

        public virtual IErrorTag CreateTag()
        {
            return new ErrorTag("Intellisense", Message);
        }
    }
}
