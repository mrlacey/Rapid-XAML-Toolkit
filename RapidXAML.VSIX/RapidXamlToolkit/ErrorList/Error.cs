using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.ErrorList
{
    public class Error
    {
        public string ExtendedMessage { get; set; }

        public string Message { get; set; }

        public SnapshotSpan Span { get; set; }
    }
}
