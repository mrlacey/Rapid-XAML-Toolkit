using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using JetBrains.ReSharper.Psi.Xml.Impl;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace ReSharperPlugin.RapidXaml;

public sealed class XamlNamespaceCodeFoldingProcessor :
    XmlTreeVisitor<FoldingHighlightingConsumer, bool>,
    ICodeFoldingProcessor
{
    public bool InteriorShouldBeProcessed(ITreeNode element, FoldingHighlightingConsumer context) => true;
    public bool IsProcessingFinished(FoldingHighlightingConsumer context) => false;

    public void ProcessBeforeInterior(ITreeNode element, FoldingHighlightingConsumer context)
    {
        if (!(element is IXmlTreeNode xamlElement))
        {
            return;
        }

        xamlElement.AcceptVisitor(this, context);
    }

    public void ProcessAfterInterior(ITreeNode element, FoldingHighlightingConsumer context)
    {
    }

    public override bool Visit(IXmlTag tag, FoldingHighlightingConsumer context)
    {
        // TODO: review siblings to check that there's nothing else between aliases

        if (tag is IXamlTypeDeclaration { NamespaceAliases.Count: > 1 } element)
        {
            var endRange = element.NamespaceAliases.Last().GetDocumentRange();
            var allRange = endRange.SetStartTo(element.NamespaceAliases.First().GetDocumentRange().StartOffset);
            context.AddLowerPriorityFolding("XAML Namespace Alias Folding", allRange, allRange, "xmlns...");
            
            return true;
        }
        
        return base.Visit(tag, context);
    }
}