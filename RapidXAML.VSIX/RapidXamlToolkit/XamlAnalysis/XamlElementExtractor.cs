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
        public static bool Parse(ITextSnapshot snapshot, string xaml, List<(string element, XamlElementProcessor processor)> processors, List<IRapidXamlAdornmentTag> tags)
        {
            var elementsOfInterest = processors.Select(p => p.element).ToList();

            var elementsBeingTracked = new List<TrackingElement>();

            bool isIdentifyingElement = false;
            bool isClosingElement = false;
            bool inLineOpeningWhitespace = true;
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
                    isIdentifyingElement = true;
                    inLineOpeningWhitespace = false;
                    currentElementStartPos = i;
                    lastElementName = currentElementName.ToString();
                    currentElementName.Clear();
                    currentElementBody = new StringBuilder("<");
                }
                else if (char.IsLetterOrDigit(xaml[i]))
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
                        if (elementsOfInterest.Contains(currentElementName.ToString()))
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
                    inLineOpeningWhitespace = false;

                    if (isIdentifyingElement)
                    {
                        if (elementsOfInterest.Contains(currentElementName.ToString()))
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
                            foreach (var (element, processor) in processors)
                            {
                                if (element == toProcess.ElementName)
                                {
                                    processor.Process(toProcess.StartPos, toProcess.ElementBody.ToString(), lineIndent.ToString(), snapshot, tags);
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

            return true;
        }

        private struct TrackingElement
        {
            public int StartPos { get; set; }

            public string ElementName { get; set; }

            public StringBuilder ElementBody { get; set; }
        }
    }
}
