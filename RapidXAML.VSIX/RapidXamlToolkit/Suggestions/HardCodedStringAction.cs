using System;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Suggestions
{
    public class HardCodedStringAction : BaseSuggestedAction
    {
        private string file;
        private ITextView view;
        public HardCodedStringTag tag;

        public override ImageMoniker IconMoniker => KnownMonikers.GenerateResource;

        public override string DisplayText
        {
            get { return "Move hard coded string to resource file."; }
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static HardCodedStringAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HardCodedStringAction
            {
                tag = tag,
                file = file,
                view = view,
            };

            return result;
        }
    }
}
