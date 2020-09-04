// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Text;
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

        // TODO: remove unused parameters
        // TODO: need to distinguish processors that need to do a contains check
        public static bool Parse(ProjectType projectType, string fileName, ITextSnapshot snapshot, string xaml, List<(string element, XamlElementProcessor processor)> processors, TagList tags, IVisualStudioAbstraction vsAbstraction, List<TagSuppression> suppressions, string projectFilePath, XamlElementProcessor everyElementProcessor, ILogger logger)
        {
            var elementsBeingTracked = new List<TrackingElement>();

            bool isIdentifyingElement = false;
            bool isClosingElement = false;
            bool inLineOpeningWhitespace = true;
            bool inComment = false;
            int currentElementStartPos = -1;

            var lastElementName = string.Empty;
            var currentElementName = new StringBuilder();
            var closingElementName = new StringBuilder();
            var lineIndent = new StringBuilder();  // Use this rather than counting characters as may be a combination of tabs and spaces

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
                    if (isIdentifyingElement)
                    {
                        elementsBeingTracked.Add(
                            new TrackingElement
                            {
                                StartPos = currentElementStartPos,
                                ElementName = currentElementName.ToString(),
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
                    isClosingElement = true;
                    closingElementName.Clear();
                    isIdentifyingElement = false;
                    inLineOpeningWhitespace = false;
                }
                else if (xaml[i] == '>')
                {
                    if (i > 2 && xaml.Substring(i - 2, 3) == "-->")
                    {
                        inComment = false;
                    }
                    else
                    {
                        inLineOpeningWhitespace = false;

                        if (isIdentifyingElement)
                        {
                            elementsBeingTracked.Add(
                                new TrackingElement
                                {
                                    StartPos = currentElementStartPos,
                                    ElementName = currentElementName.ToString(),
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

                            for (int j = elementsBeingTracked.Count - 1; j >= 0; j--)
                            {
                                if (elementsBeingTracked[j].ElementName == nameOfInterest)
                                {
                                    toProcess = elementsBeingTracked[j];
                                    break;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(toProcess.ElementName))
                            {
                                var elementBody = xaml.Substring(toProcess.StartPos, i - toProcess.StartPos + 1);

                                if (elementBody.StartsWith("</"))
                                {
                                    System.Diagnostics.Debug.WriteLine("DEBUG!!!!!!");
                                }

                                // TODO: Is this also the place to check for processors based on contains?
                                everyElementProcessor?.Process(fileName, toProcess.StartPos, elementBody, lineIndent.ToString(), snapshot, tags, suppressions);

                                for (int j = 0; j < processors.Count; j++)
                                {
                                    if (processors[j].element == toProcess.ElementName
                                     || processors[j].element == toProcess.ElementNameWithoutNamespace)
                                    {
                                        try
                                        {
                                            processors[j].processor.Process(fileName, toProcess.StartPos, elementBody, lineIndent.ToString(), snapshot, tags, suppressions);
                                        }
                                        catch (Exception exc)
                                        {
                                            var bubbleUpError = true;

                                            if (processors[j].processor is CustomProcessorWrapper wrapper)
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
                                    else if (processors[j].element.StartsWith(AnyContainingStart, System.StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (elementBody.Contains(processors[j].element.Substring(AnyContainingStart.Length)))
                                        {
                                            processors[j].processor.Process(fileName, toProcess.StartPos, elementBody, lineIndent.ToString(), snapshot, tags, suppressions);
                                        }
                                    }
                                }

                                elementsBeingTracked.Remove(toProcess);
                            }
                            else
                            {
                                if (!inComment)
                                {
                                    var elementBody = xaml.Substring(currentElementStartPos, i - currentElementStartPos + 1);

                                    // Don't process closing blocks
                                    if (!elementBody.StartsWith("</"))
                                    {
                                        // Do this in the else so don't always have to calculate the substring.
                                        everyElementProcessor?.Process(fileName, currentElementStartPos, elementBody, lineIndent.ToString(), snapshot, tags, suppressions);
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
                    };
                }
            }

            public int StartPos { get; set; }

            public string ElementName { get; set; }

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
