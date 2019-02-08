using System;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.Tagging
{
    public class ParsingEventArgs : EventArgs
    {
        public ParsingEventArgs(XamlDocument document, string file, ITextSnapshot snapshot)
        {
            Document = document;
            File = file;
            Snapshot = snapshot;
        }

        public XamlDocument Document { get; set; }

        public string File { get; set; }

        public ITextSnapshot Snapshot { get; set; }
    }
}
