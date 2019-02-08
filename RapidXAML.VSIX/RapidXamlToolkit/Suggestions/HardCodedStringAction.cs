using System;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;

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
                // linkUrl = errorTag.Url,
                tag = tag,
                file = file,
                view = view,
            };

            return result;
        }
    }
}
