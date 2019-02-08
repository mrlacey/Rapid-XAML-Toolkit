using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Suggestions
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType(KnownContentTypes.Xaml)]
    public class RapidXamlTagProvider : ITaggerProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            ITextDocument document;

            if (!TextDocumentFactoryService.TryGetTextDocument(buffer, out document))
            {
                return null;
            }

            return buffer.Properties.GetOrCreateSingletonProperty(() => new RapidXamlTagger(buffer, document.FilePath)) as ITagger<T>;
        }
    }
}
