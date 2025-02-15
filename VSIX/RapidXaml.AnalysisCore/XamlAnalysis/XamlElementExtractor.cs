// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class XamlElementExtractor
    {
        private const string AnyContainingStart = "ANYCONTAINING:";
        private const string AnyOrChildrenContainingStart = "ANYORCHILDRENCONTAINING:";

        public static bool Parse(string fileName, ITextSnapshotAbstraction snapshot, string xaml, List<(string Element, XamlElementProcessor Processor)> processors, TagList tags, List<TagSuppression> suppressions, XamlElementProcessor everyElementProcessor, ILogger logger)
        {
            var elementsBeingTracked = new Stack<TrackingElement>();

            bool isIdentifyingElement = false;
            bool isClosingElement = false;
            bool inLineOpeningWhitespace = true;
            bool inComment = false;
            int currentElementStartPos = -1;

            var lastElementName = string.Empty;
            var currentElementName = new StringBuilder();
            var closingElementName = new StringBuilder();
            var lineIndent = new StringBuilder();  // Use this rather than counting characters as may be a combination of tabs and spaces

            Dictionary<string, string> xmlnsAliases = null;

            for (int i = 0; i < xaml.Length; i++)
            {
                if (xaml[i] == '<')
                {
                    if (!inComment)
                    {
                        isIdentifyingElement = true;
                        inLineOpeningWhitespace = false;
                        currentElementStartPos = i;
                        lastElementName = currentElementName.ToString();
                        currentElementName.Clear();
                    }
                }
                else if (char.IsLetterOrDigit(xaml[i]) || xaml[i] == ':' || xaml[i] == '_')
                {
                    if (!inComment)
                    {
                        if (isIdentifyingElement)
                        {
                            currentElementName.Append(xaml[i]);
                        }
                        else if (isClosingElement)
                        {
                            closingElementName.Append(xaml[i]);
                        }
                    }

                    inLineOpeningWhitespace = false;
                }
                else if (xaml[i] == '\r' || xaml[i] == '\n')
                {
                    if (!isIdentifyingElement)
                    {
                        lineIndent.Clear();
                        inLineOpeningWhitespace = true;
                    }
                }
                else if (char.IsWhiteSpace(xaml[i]))
                {
                    if (isIdentifyingElement && currentElementName.Length > 0)
                    {
                        elementsBeingTracked.Push(
                            new TrackingElement
                            {
                                StartPos = currentElementStartPos,
                                ElementName = currentElementName.ToString(),
                                FirstChildPos = -1,
                            });
                    }

                    if (inLineOpeningWhitespace)
                    {
                        lineIndent.Append(xaml[i]);
                    }

                    isIdentifyingElement = false;
                }
                else if (xaml[i] == '/')
                {
                    // Also need to look at being here as part of a URL (such as in an xmlns declaration)
                    isClosingElement = true;
                    closingElementName.Clear();
                    isIdentifyingElement = false;
                    inLineOpeningWhitespace = false;
                }
                else if (xaml[i] == '>')
                {
                    if (i > 2 && (xaml.Substring(i - 2, 3) == "-->" || xaml.Substring(i - 1, 2) == "?>"))
                    {
                        inComment = false;
                    }
                    else
                    {
                        if (xmlnsAliases == null)
                        {
                            xmlnsAliases = new Dictionary<string, string>();

                            var props = xaml.Substring(currentElementStartPos, i - currentElementStartPos).Split(new[] { " ", "\t", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < props.Length; j++)
                            {
                                string prop = props[j];

                                // Nest these calls to avoid having to always do both
                                if (prop.StartsWith("xmlns", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var equalsIndex = prop.IndexOf("=");

                                    if (prop.StartsWith("xmlns:", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        while (!prop.Contains("=") || (!prop.TrimEnd().EndsWith("\"") && !prop.TrimEnd().EndsWith("'")))
                                        {
                                            prop += props[++j].Trim();
                                        }

                                        equalsIndex = prop.IndexOf("=");

                                        xmlnsAliases.Add(prop.Substring(6, equalsIndex - 6), prop.Substring(equalsIndex + 1).Trim('"', '\''));
                                    }
                                    else
                                    {
                                        xmlnsAliases.Add(string.Empty, prop.Substring(equalsIndex + 1).Trim('"', '\''));
                                    }
                                }
                            }
                        }

                        inLineOpeningWhitespace = false;

                        if (isIdentifyingElement && currentElementName.Length > 0)
                        {
                            elementsBeingTracked.Push(
                                new TrackingElement
                                {
                                    StartPos = currentElementStartPos,
                                    ElementName = currentElementName.ToString(),
                                    FirstChildPos = -1,
                                });

                            isIdentifyingElement = false;
                        }

                        // closing blocks can be blank or named (e.g. ' />' or '</Grid>')
                        if (isClosingElement)
                        {
                            var nameOfInterest = closingElementName.ToString();

                            if (string.IsNullOrWhiteSpace(nameOfInterest))
                            {
                                nameOfInterest = currentElementName.ToString();
                            }
                            else if (nameOfInterest == lastElementName)
                            {
                                nameOfInterest = lastElementName;
                            }

                            var toProcess = TrackingElement.Default;

                            if (elementsBeingTracked.Count > 0
                             && elementsBeingTracked.Peek().ElementName == nameOfInterest)
                            {
                                toProcess = elementsBeingTracked.Pop();
                            }

                            // This might not be the case if the was an encoded opening but not an encoded closing angle bracket
                            if (!string.IsNullOrWhiteSpace(toProcess.ElementName))
                            {
                                var elementBody = xaml.Substring(toProcess.StartPos, i - toProcess.StartPos + 1);

                                // Track first child pos as much cheaper than calling GetOpeningWithoutChildren for every element
                                if (elementsBeingTracked.Count > 0
                                 && elementsBeingTracked.Peek().FirstChildPos == -1)
                                {
                                    var replacement = elementsBeingTracked.Pop();

                                    elementsBeingTracked.Push(new TrackingElement
                                    {
                                        ElementName = replacement.ElementName,
                                        StartPos = replacement.StartPos,
                                        FirstChildPos = toProcess.StartPos,
                                    });
                                }

                                string xamlElementWithoutChildren;

                                if (toProcess.FirstChildPos == -1)
                                {
                                    // Remove children to avoid getting duplicates when children are processed.
                                    xamlElementWithoutChildren = XamlElementProcessor.GetOpeningWithoutChildren(elementBody);
                                }
                                else
                                {
                                    xamlElementWithoutChildren = xaml.Substring(toProcess.StartPos, toProcess.FirstChildPos - toProcess.StartPos);
                                }

                                everyElementProcessor?.Process(fileName, toProcess.StartPos, xamlElementWithoutChildren, lineIndent.ToString(), snapshot, tags, suppressions, xmlnsAliases);

                                // Avoid calculating these for every processor
                                var elementName = toProcess.ElementName;
                                var elementNameWithoutNamespace = toProcess.ElementNameWithoutNamespace;

                                for (int j = 0; j < processors.Count; j++)
                                {
                                    if (processors[j].Element == elementName
                                     || processors[j].Element == elementNameWithoutNamespace)
                                    {
                                        try
                                        {
                                            processors[j].Processor.Process(fileName, toProcess.StartPos, elementBody, lineIndent.ToString(), snapshot, tags, suppressions, xmlnsAliases);
                                        }
                                        catch (Exception exc)
                                        {
                                            var bubbleUpError = true;

                                            if (processors[j].Processor is CustomProcessorWrapper wrapper)
                                            {
                                                var customAnalyzer = wrapper.CustomAnalyzer;

                                                if (!(customAnalyzer is BuiltInXamlAnalyzer))
                                                {
                                                    logger?.RecordError(StringRes.Error_ErrorInCustomAnalyzer.WithParams(customAnalyzer.GetType().FullName), force: true);
                                                    logger?.RecordError(StringRes.Error_ErrorInCustomAnalyzer.WithParams(customAnalyzer.GetType().FullName));
                                                    logger?.RecordException(exc);
                                                    bubbleUpError = false;
                                                }
                                            }

                                            if (bubbleUpError)
                                            {
                                                throw;
                                            }
                                        }
                                    }
                                    else if (processors[j].Element.StartsWith("ANY", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        // These two calls will very rarely be true.
                                        // The above single check is to avoid two unlikely checks.
                                        if (processors[j].Element.StartsWith(AnyContainingStart, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (XamlElementProcessor.GetOpeningWithoutChildren(elementBody).Contains(processors[j].Element.Substring(AnyContainingStart.Length)))
                                            {
                                                processors[j].Processor.Process(fileName, toProcess.StartPos, elementBody, lineIndent.ToString(), snapshot, tags, suppressions, xmlnsAliases);
                                            }
                                        }
                                        else if (processors[j].Element.StartsWith(AnyOrChildrenContainingStart, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (elementBody.Contains(processors[j].Element.Substring(AnyOrChildrenContainingStart.Length)))
                                            {
                                                processors[j].Processor.Process(fileName, toProcess.StartPos, elementBody, lineIndent.ToString(), snapshot, tags, suppressions, xmlnsAliases);
                                            }
                                        }
                                    }
                                }
                            }

                            // Reset this so know what we should be tracking
                            currentElementStartPos = -1;
                            isClosingElement = false;
                        }
                    }
                }
                else if (xaml[i] == '-')
                {
                    if (i >= 3 && xaml.Substring(i - 3, 4) == "<!--")
                    {
                        inComment = true;
                    }
                }
                else if (xaml[i] == '?')
                {
                    if (i >= 2 && xaml.Substring(i - 1, 2) == "<?")
                    {
                        inComment = true;
                    }
                }
            }

            return true;
        }

        private struct TrackingElement
        {
            public static TrackingElement Default
            {
                get
                {
                    return new TrackingElement
                    {
                        StartPos = int.MaxValue,
                        ElementName = string.Empty,
                        FirstChildPos = -1,
                    };
                }
            }

            public int StartPos { get; set; }

            public string ElementName { get; set; }

            public int FirstChildPos { get; set; }

            public string ElementNameWithoutNamespace
            {
                get
                {
                    return this.ElementName?.AsSpan().PartAfter(':') ?? string.Empty;
                }
            }
        }
    }
}
