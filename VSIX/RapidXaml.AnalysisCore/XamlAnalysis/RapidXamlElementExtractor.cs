// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.Language.Xml;
using RapidXaml;
using RapidXaml.TestHelpers;

namespace RapidXamlToolkit.XamlAnalysis
{
    // This is internal as also linked into the CustomAnalysis & AutoFix projects.
    internal static class RapidXamlElementExtractor
    {
        // This is quite a simple caching strategy.
        // Ideally would like to avoid discarding items from the cache that are recently used.
        private static readonly SizeLimitedDictionary<string, RapidXamlElement> RxElementCache
            = new SizeLimitedDictionary<string, RapidXamlElement>(200);

        // This doesn't have the extra input checking that is in the caller in the TestHelper.
        // Extra checks excluded here as input is already known good based on original doc parsing.
        public static RapidXamlElement GetElement(string xamlElement, int offset = 0)
        {
            RapidXamlElement GetElementInternal(string xaml, int startOffset)
            {
                if (string.IsNullOrWhiteSpace(xaml))
                {
                    return null;
                }

                var docSyntax = Parser.ParseText(xaml);

                var xdoc = docSyntax?.RootSyntax;

                if (xdoc == null)
                {
                    return null;
                }

                var elementName = xdoc.Name;

                var result = RapidXamlElement.Build(
                    elementName,
                    startOffset + docSyntax.SpanStart,
                    docSyntax.Width,
                    xaml);

                var content = (docSyntax.Body as IXmlElement).Value;

                foreach (var attr in xdoc.Attributes)
                {
                    result.AddInlineAttribute(attr.Name, attr.Value, startOffset + attr.SpanStart, attr.Width);
                }

                foreach (var child in docSyntax.Body.ChildNodes)
                {
                    if (child == null | child is XmlElementStartTagSyntax | child is XmlElementEndTagSyntax)
                    {
                        continue;
                    }

                    if (child is XmlElementSyntax childElement)
                    {
                        if (childElement.Name.StartsWith($"{elementName}."))
                        {
                            var fullspan = childElement.Content.FullSpan;
                            var attrString = xaml.Substring(fullspan.Start, fullspan.Length);

                            if (attrString.TrimStart().StartsWith("<"))
                            {
                                var startingWhiteSpaceLength = attrString.IndexOf("<");

                                foreach (var innerChild in childElement.ChildNodes)
                                {
                                    if (innerChild == null | innerChild is XmlElementStartTagSyntax | innerChild is XmlElementEndTagSyntax)
                                    {
                                        continue;
                                    }

                                    if (innerChild is SyntaxList childList)
                                    {
                                        var attributeChildren = new List<RapidXamlElement>();

                                        foreach (SyntaxNode listItem in childList.ChildNodes)
                                        {
                                            if (!(listItem is XmlCommentSyntax))
                                            {
                                                attributeChildren.Add(
                                                    GetElementInternal(
                                                        xaml.Substring(listItem.SpanStart, listItem.Width),
                                                        startOffset + listItem.SpanStart));
                                            }
                                        }

                                        result.AddChildrenAttribute(
                                            childElement.Name.Substring(elementName.Length + 1),
                                            attributeChildren,
                                            startOffset + childElement.SpanStart,
                                            childElement.Width);
                                    }
                                    else
                                    {
                                        var innerString = xaml.Substring(innerChild.SpanStart, innerChild.Width);

                                        result.AddChildAttribute(
                                            childElement.Name.Substring(elementName.Length + 1),
                                            GetElementInternal(innerString, startOffset + startingWhiteSpaceLength + innerChild.Start),
                                            startOffset + childElement.SpanStart,
                                            childElement.Width);
                                    }
                                }
                            }
                            else
                            {
                                result.AddChildAttribute(
                                    childElement.Name.Substring(elementName.Length + 1),
                                    attrString,
                                    startOffset + childElement.SpanStart,
                                    childElement.Width);
                            }

                            var childAsString = xaml.TrimStart().Substring(childElement.SpanStart, childElement.Width);

                            if (content.TrimStart().StartsWith(childAsString.TrimStart()))
                            {
                                content = content.TrimStart().Substring(childAsString.Length).Trim();
                            }
                        }
                        else
                        {
                            result.AddChild(GetElementInternal(content, startOffset + child.Start));
                        }
                    }
                    else if (child is XmlEmptyElementSyntax selfClosingChild)
                    {
                        var toAdd =
                            RapidXamlElement.Build(
                                selfClosingChild.Name,
                                startOffset + child.SpanStart,
                                child.Width,
                                xaml.Substring(child.SpanStart, child.Width));

                        foreach (var attr in selfClosingChild.AttributesNode)
                        {
                            toAdd.AddInlineAttribute(attr.Name, attr.Value, startOffset + attr.SpanStart, attr.Width);
                        }

                        result.AddChild(toAdd);
                    }
                    else if (child is SyntaxNode node)
                    {
                        foreach (var nodeChild in node.ChildNodes)
                        {
                            if (nodeChild is XmlTextTokenSyntax)
                            {
                                continue;
                            }

                            if (nodeChild is XmlElementSyntax ncElement)
                            {
                                if (ncElement.Name.StartsWith($"{elementName}."))
                                {
                                    var attrString = xaml.Substring(ncElement.Content.Span.Start, ncElement.Content.Span.Length);

                                    if (attrString.StartsWith("<"))
                                    {
                                        foreach (var attrChild in ncElement.ChildNodes)
                                        {
                                            if (attrChild is XmlElementSyntax || attrChild is XmlEmptyElementSyntax)
                                            {
                                                result.AddChildAttribute(
                                                    ncElement.Name.Substring(elementName.Length + 1),
                                                    GetElementInternal(attrString, startOffset + ncElement.Content.Span.Start),
                                                    startOffset + ncElement.SpanStart,
                                                    ncElement.Width);
                                            }
                                            else if (attrChild is SyntaxList acList)
                                            {
                                                var children = new List<RapidXamlElement>();

                                                foreach (var acListChild in acList.ChildNodes)
                                                {
                                                    if (acListChild is XmlCommentSyntax)
                                                    {
                                                        // Don't add a null child entry for comments (as comments are currently ignored)
                                                        continue;
                                                    }

                                                    var newElement = GetElementInternal(
                                                            xaml.Substring(acListChild.SpanStart, acListChild.Width),
                                                            startOffset + acListChild.SpanStart);

                                                    if (newElement != null)
                                                    {
                                                        children.Add(newElement);
                                                    }
                                                }

                                                result.AddChildrenAttribute(
                                                    ncElement.Name.Substring(elementName.Length + 1),
                                                    children,
                                                    startOffset + ncElement.SpanStart,
                                                    ncElement.Width);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        result.AddChildAttribute(
                                            ncElement.Name.Substring(elementName.Length + 1),
                                            attrString,
                                            startOffset + ncElement.SpanStart,
                                            ncElement.Width);
                                    }

                                    var childAsString = xaml.Substring(ncElement.Start, ncElement.Width);

                                    if (content.TrimStart().StartsWith(childAsString.TrimStart()))
                                    {
                                        content = content.TrimStart().Substring(childAsString.Length).Trim();
                                    }
                                }
                                else
                                {
                                    result.AddChild(GetElementInternal(xaml.Substring(nodeChild.SpanStart, nodeChild.Width), startOffset + nodeChild.SpanStart));
                                }
                            }
                            else if (nodeChild is XmlEmptyElementSyntax ncSelfClosing)
                            {
                                var nodeToAdd =
                                    RapidXamlElement.Build(
                                        ncSelfClosing.Name,
                                        startOffset + nodeChild.SpanStart,
                                        nodeChild.Width,
                                        xaml.Substring(nodeChild.SpanStart, nodeChild.Width));

                                foreach (var attr in ncSelfClosing.AttributesNode)
                                {
                                    nodeToAdd.AddInlineAttribute(attr.Name, attr.Value, startOffset + attr.SpanStart, attr.Width);
                                }

                                result.AddChild(nodeToAdd);
                            }
                        }
                    }
                }

                result.SetContent(content);

                return result;
            }

            //// Cache these responses to avoid unnecessary repeated parsing
            if (!string.IsNullOrWhiteSpace(xamlElement)
                && RxElementCache.ContainsKey(xamlElement))
            {
                return RxElementCache[xamlElement].WithUpdatedLocationStart(offset);
            }
            else
            {
                var rxElement = GetElementInternal(xamlElement, offset);

                if (rxElement != null)
                {
                    RxElementCache.Add(xamlElement, rxElement);
                }

                return rxElement;
            }
        }
    }
}
