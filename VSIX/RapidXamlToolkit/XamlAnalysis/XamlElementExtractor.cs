// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class XamlElementExtractor
    {
        public static bool Parse(ProjectType projectType, string fileName, ITextSnapshot snapshot, string xaml, List<(string element, XamlElementProcessor processor)> processors, TagList tags, List<TagSuppression> suppressions = null)
        {
            var elementsOfInterest = processors.Select(p => p.element).ToList();

            var elementsBeingTracked = new List<TrackingElement>();

            var everyElementProcessor = new EveryElementProcessor(projectType, RapidXamlPackage.Logger);

            bool isIdentifyingElement = false;
            bool isClosingElement = false;
            bool inLineOpeningWhitespace = true;
            bool inComment = false;
            int currentElementStartPos = -1;

            var lastElementName = string.Empty;
            var currentElementName = new StringBuilder();
            var currentElementBody = new StringBuilder();
            var closingElementName = new StringBuilder();
            var lineIndent = new StringBuilder();

            for (int i = 0; i < xaml.Length; i++)
            {
                currentElementBody.Append(xaml[i]);

                for (var j = 0; j < elementsBeingTracked.Count; j++)
                {
                    elementsBeingTracked[j].ElementBody.Append(xaml[i]);
                }

                if (xaml[i] == '<')
                {
                    if (!inComment)
                    {
                        isIdentifyingElement = true;
                        inLineOpeningWhitespace = false;
                        currentElementStartPos = i;
                        lastElementName = currentElementName.ToString();
                        currentElementName.Clear();
                        currentElementBody = new StringBuilder("<");
                    }
                }
                else if (char.IsLetterOrDigit(xaml[i]) || xaml[i] == ':')
                {
                    if (isIdentifyingElement)
                    {
                        currentElementName.Append(xaml[i]);
                    }
                    else if (isClosingElement)
                    {
                        closingElementName.Append(xaml[i]);
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
                        if (elementsOfInterest.Contains(currentElementName.ToString().PartAfter(":")))
                        {
                            elementsBeingTracked.Add(
                                new TrackingElement
                                {
                                    StartPos = currentElementStartPos,
                                    ElementName = currentElementName.ToString(),
                                    ElementBody = new StringBuilder(currentElementBody.ToString()),
                                });
                        }
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
                            if (elementsOfInterest.Contains(currentElementName.ToString().PartAfter(":")))
                            {
                                elementsBeingTracked.Add(
                                    new TrackingElement
                                    {
                                        StartPos = currentElementStartPos,
                                        ElementName = currentElementName.ToString(),
                                        ElementBody = new StringBuilder(currentElementBody.ToString()),
                                    });
                            }

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

                            var toProcess = elementsBeingTracked.Where(g => g.ElementName == nameOfInterest)
                                                                .OrderByDescending(f => f.StartPos)
                                                                .Select(e => e)
                                                                .FirstOrDefault();

                            if (!string.IsNullOrWhiteSpace(toProcess.ElementName))
                            {
                                everyElementProcessor.Process(fileName, toProcess.StartPos, toProcess.ElementBody.ToString(), lineIndent.ToString(), snapshot, tags, suppressions);

                                foreach (var (element, processor) in processors)
                                {
                                    if (element == toProcess.ElementNameWithoutNamespace)
                                    {
                                        processor.Process(fileName, toProcess.StartPos, toProcess.ElementBody.ToString(), lineIndent.ToString(), snapshot, tags, suppressions);
                                    }
                                }

                                elementsBeingTracked.Remove(toProcess);
                            }

                            // Reset this so know what we should be tracking
                            currentElementStartPos = -1;
                            isClosingElement = false;
                        }
                    }
                }
                else if (xaml[i] == '-')
                {
                    if (i > 3 && xaml.Substring(i - 3, 4) == "<!--")
                    {
                        inComment = true;
                    }
                }
            }

            return true;
        }

        private struct TrackingElement
        {
            public int StartPos { get; set; }

            public string ElementName { get; set; }

            public string ElementNameWithoutNamespace
            {
                get
                {
                    return this.ElementName.PartAfter(":");
                }
            }

            public StringBuilder ElementBody { get; set; }
        }
    }
}
