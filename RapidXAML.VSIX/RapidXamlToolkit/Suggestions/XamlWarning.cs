using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.Tagging
{
    public class XamlWarning
    {
        public readonly SnapshotSpan Span;

        // This is used by ErrorsSnapshot.TranslateTo() to map this error to the corresponding error in the next snapshot.
        public int NextIndex = -1;

        public XamlWarning(SnapshotSpan span)
        {
            this.Span = span;
        }

        public static XamlWarning Clone(XamlWarning error)
        {
            return new XamlWarning(error.Span);
        }

        public static XamlWarning CloneAndTranslateTo(XamlWarning error, ITextSnapshot newSnapshot)
        {
            var newSpan = error.Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            // We want to only translate the error if the length of the error span did not change (if it did change, it would imply that
            // there was some text edit inside the error and, therefore, that the error is no longer valid).
            return (newSpan.Length == error.Span.Length)
                ? new XamlWarning(newSpan)
                : null;
        }
    }
}
