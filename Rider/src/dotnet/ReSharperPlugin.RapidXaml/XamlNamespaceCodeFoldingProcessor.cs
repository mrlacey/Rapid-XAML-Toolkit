using System;
using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.Impl;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace ReSharperPlugin.RapidXaml;

public sealed class XamlNamespaceCodeFoldingProcessor :
    XmlTreeVisitor<FoldingHighlightingConsumer, bool>,
    ICodeFoldingProcessor
{
    private readonly Stack<(DocumentOffset offset, string regionText)> _regionStarts =
        new Stack<(DocumentOffset offset, string regionText)>();

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
        System.Diagnostics.Debug.WriteLine(tag);
       
        // If is IXamlTypeDeclaration
        // If has multiple NamespaceAliases
        // place fold start at the start position of the first
        // place fold end at the end of the last one
        
        // TODO: use tag.GetDocumentRange() to review lines that shouldn't be included
        // ((JetBrains.ReSharper.Psi.Xaml.Impl.Tree.XamlTypeDeclaration) tag).NamespaceAliases.myNodes[1].GetDocumentRange()
        // TODO: review siblings to check that there's nothing else between aliases
        
        return base.Visit(tag, context);
    }

    public override bool Visit(IXmlDocumentNode document, FoldingHighlightingConsumer context)
    {
        System.Diagnostics.Debug.WriteLine(document);
        return base.Visit(document, context);
    }

    public override bool Visit(IXmlCommentNode xmlComment, FoldingHighlightingConsumer context)
    {
        var commentText = xmlComment.CommentText.Trim();
        if (IsRegionStart(commentText, out var regionText))
        {
            _regionStarts.Push((xmlComment.GetDocumentRange().StartOffset, regionText));
            return true;
        }

        if (IsRegionEnd(commentText))
        {
            if (_regionStarts.Count > 0)
            {
                var (regionStartOffset, placeholder) = _regionStarts.Pop();
                var regionEndRange = xmlComment.GetDocumentRange();
                var regionRange = regionEndRange.SetStartTo(regionStartOffset);
                context.AddLowerPriorityFolding("XAML Region Regions Folding", regionRange, regionRange,
                    placeholder);
                return true;
            }

            // endregion without start
            return false;
        }

        return false;
    }

    private const string RegionStartTag = "#region";
    private const string RegionEndTag = "#endregion";

    private static bool IsRegionEnd(string commentText)
    {
        return (commentText.Contains(RegionEndTag));
    }

    private static bool IsRegionStart(string commentText, out string regionText)
    {
        if (commentText.StartsWith(RegionStartTag))
        {
            regionText = commentText
                .Substring(Math.Min(commentText.Length, RegionStartTag.Length + 1))
                .Trim();
            if (string.IsNullOrWhiteSpace(regionText))
            {
                regionText = "#region";
            }

            return true;
        }

        regionText = null;
        return false;
    }
}