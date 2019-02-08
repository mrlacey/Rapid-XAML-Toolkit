using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
    public class RapidXamlDocument
    {
        public RapidXamlDocument()
        {
            this.SuggestionTags = new List<IRapidXamlTag>();
        }

        public string RawText { get; set; }

        public List<IRapidXamlTag> SuggestionTags { get; set; }

        public static RapidXamlDocument Create(ITextSnapshot snapshot)
        {
            var result = new RapidXamlDocument();

            try
            {
                var text = snapshot.GetText();
                result.RawText = text;

                // TODO: offload the creation of tags to separate classes for handling each XAML element
                // Register handlers and the elements they are looking for

                // walk the file until find an element that's registered
                // Keep walking until get to the end of that element
                // pass the text of the element (and position offset) to the handler
                // handler processes the passed text and may add items to the SuggestionTags



                var count = 0;

                var rowDefIndex = text.IndexOf("<RowDefinition");
                while (rowDefIndex >= 0)
                {
                    var endPos = text.IndexOf('>', rowDefIndex);

                    var tag = new InsertRowDefinitionTag
                    {
                        Span = new Span(rowDefIndex, endPos - rowDefIndex),
                        RowId = count,
                    };

                    result.SuggestionTags.Add(tag);

                    count = count + 1;

                    rowDefIndex = text.IndexOf("<RowDefinition", endPos);
                }


                var tbIndex = text.IndexOf("<TextBlock Text=\"");

                if (tbIndex >= 0)
                {
                    var tbEnd = text.IndexOf(">", tbIndex);

                    var line = snapshot.GetLineFromPosition(tbIndex);
                    var col = tbEnd - line.Start.Position;

                    result.SuggestionTags.Add(new HardCodedStringTag
                    {
                        Span = new Span(tbIndex, tbEnd - tbIndex),
                        Line = line.LineNumber,
                        Column = col,
                        Snapshot = snapshot,
                    });
                }
            }
            catch (Exception e)
            {
                result.SuggestionTags.Add(new UnexpectedErrorTag
                {
                    Span = new Span(0, 0),
                    Message = "Unexpected error occurred while parsing XAML. Please log an issue to https://github.com/Microsoft/Rapid-XAML-Toolkit/issues Reason: " + e,
                });
            }

            return result;
        }
    }

    public static class XamlElementExtractor
    {
        private struct TrackingElement
        {
            public int StartPos { get; set; }

            public string ElementName { get; set; }

            public string ElementBody { get; set; }
        }

        public static bool Parse(string xaml, List<(string element, XamlElementProcessor processor)> processors, List<IRapidXamlTag> tags)
        {
            var elementsOfInterest = processors.Select(p => p.element).ToList();

            var elementsBeingTracked = new List<TrackingElement>();

            // walk the file until find an element that's registered
            // Keep walking until get to the end of that element
            // pass the text of the element (and position offset) to the handler
            // handler processes the passed text and may add items to the SuggestionTags

            bool isIdentifyingElement = false;
            bool isClosingElement = false;
            int currentElementStartPos = -1;
            string lastElementName = String.Empty;
            string currentElementName = string.Empty;
            string currentElementBody = string.Empty;
            string closingElementName = string.Empty;

            for (int i = 0; i < xaml.Length; i++)
            {
                if (xaml[i] == '<')
                {
                    isIdentifyingElement = true;
                    currentElementStartPos = i;
                    lastElementName = currentElementName;
                    currentElementName = string.Empty;
                    currentElementBody = "<";

                    for (var j = 0; j < elementsBeingTracked.Count; j++)
                    {
                     //   elementsBeingTracked[j].ElementBody += xaml[i];

                        var ebt = elementsBeingTracked[j];
                        ebt.ElementBody += xaml[i];
                        elementsBeingTracked[j] = ebt;
                    }
                }
                else if (char.IsLetterOrDigit(xaml[i]))
                {
                    if (isIdentifyingElement)
                    {
                        currentElementName += xaml[i];
                    }
                    else if (isClosingElement)
                    {
                        closingElementName += xaml[i];
                    }

                    currentElementBody += xaml[i];  // not sure if need this

                    for (var j = 0; j < elementsBeingTracked.Count; j++)
                    {
                        var ebt = elementsBeingTracked[j];
                        ebt.ElementBody += xaml[i];
                        elementsBeingTracked[j] = ebt;
                    }
                }
                else if (char.IsWhiteSpace(xaml[i]))
                {
                    if (isIdentifyingElement)
                    {
                        if (elementsOfInterest.Contains(currentElementName))
                        {
                            elementsBeingTracked.Add(new TrackingElement { StartPos = currentElementStartPos, ElementName = currentElementName, ElementBody = currentElementBody });
                        }
                    }

                    isIdentifyingElement = false;

                    for (var j = 0; j < elementsBeingTracked.Count; j++)
                    {
                        var ebt = elementsBeingTracked[j];
                        ebt.ElementBody += xaml[i];
                        elementsBeingTracked[j] = ebt;
                    }
                }
                else if (xaml[i] == '/')
                {
                    isClosingElement = true;
                    isIdentifyingElement = false;

                    currentElementBody += xaml[i];
                    for (var j = 0; j < elementsBeingTracked.Count; j++)
                    {
                        var ebt = elementsBeingTracked[j];
                        ebt.ElementBody += xaml[i];
                        elementsBeingTracked[j] = ebt;
                    }
                }
                else if (xaml[i] == '>')
                {
                    if (isIdentifyingElement)
                    {
                        if (elementsOfInterest.Contains(currentElementName))
                        {
                            elementsBeingTracked.Add( new TrackingElement{StartPos = currentElementStartPos, ElementName = currentElementName, ElementBody = currentElementBody });
                        }

                        isIdentifyingElement = false;
                    }

                    if (isClosingElement)
                    {
                        // closing blocks can be blank or named (e.g. ' />' or '</Grid>')
                        if (string.IsNullOrEmpty(closingElementName) || closingElementName == lastElementName)
                        {
                            var toProcess = elementsBeingTracked.Where(g => g.ElementName == lastElementName)
                                                                .OrderByDescending(f => f.StartPos)
                                                                .Select(e => e)
                                                                .FirstOrDefault();

                            if (!string.IsNullOrWhiteSpace(toProcess.ElementName))
                            {
                                foreach (var p in processors)
                                {
                                    if (p.element == toProcess.ElementName)
                                    {
                                        p.processor.Process(toProcess.StartPos, toProcess.ElementBody, tags);
                                    }
                                }
                            }
                        }

                        // Reset this so know what we should be tracking
                        currentElementStartPos = -1;
                        isClosingElement = false;
                    }

                    for (var j = 0; j < elementsBeingTracked.Count; j++)
                    {
                        var ebt = elementsBeingTracked[j];
                        ebt.ElementBody += xaml[i];
                        elementsBeingTracked[j] = ebt;
                    }
                }
                else
                {
                    if (currentElementStartPos >= 0)
                    {
                        currentElementBody += xaml[i];
                        for (var j = 0; j < elementsBeingTracked.Count; j++)
                        {
                            var ebt = elementsBeingTracked[j];
                            ebt.ElementBody += xaml[i];
                            elementsBeingTracked[j] = ebt;
                        }
                    }
                }
            }

            return true;
        }
    }
}
