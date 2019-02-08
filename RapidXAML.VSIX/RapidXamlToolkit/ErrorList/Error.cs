using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.ErrorList
{
    public class ErrorRow
    {
        public string ExtendedMessage { get; set; }

        public string Message { get; set; }

        public SnapshotSpan Span { get; set; }
    }
}
