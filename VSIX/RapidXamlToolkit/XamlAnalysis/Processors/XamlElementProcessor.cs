// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public abstract class XamlElementProcessor
    {
        public static bool IsSelfClosing(string xaml, int startPoint = 0)
        {
            var foundSelfCloser = false;

            for (var i = startPoint; i < xaml.Length; i++)
            {
                switch (xaml[i])
                {
                    case '/':
                        foundSelfCloser = true;
                        break;
                    case '>':
                        return foundSelfCloser;
                    default:
                        break;
                }
            }

            // Shouldn't ever get here if passed valid XAML and startPoint is valid
            return false;
        }

        public static Dictionary<int, int> GetExclusions(string xaml, string elementName)
        {
            string elementOpen = $"<{elementName}";
            string elementOpenSpace = $"<{elementName} ";
            string elementOpenComplete = $"<{elementName}>";
            string elementClose = $"</{elementName}>";

            var exclusions = new Dictionary<int, int>();

            // This is the opening position of the next opening (here) or closing (when set subsequently) tag
            var tagOfInterestPos = xaml.Substring(elementOpen.Length).FirstIndexOf(elementOpenComplete, elementOpenSpace) + elementOpen.Length;

            // track the number of open tags seen so know when get to the corresponding closing one.
            var openings = 0;

            // Track this outside the loop as may have nesting.
            int startClosePos = 0;

            while (tagOfInterestPos > elementOpen.Length && tagOfInterestPos < xaml.Length)
            {
                // closing tags
                if (xaml.Substring(tagOfInterestPos, 2) == "</")
                {
                    // Allow for having seen multiple openings before the closing
                    if (openings == 1)
                    {
                        exclusions.Add(startClosePos + 1, tagOfInterestPos + elementClose.Length);
                        openings = 0;
                    }
                    else
                    {
                        openings -= 1;
                    }
                }
                else
                {
                    // ignore self closing tags as nothing to exclude
                    if (!XamlElementProcessor.IsSelfClosing(xaml, tagOfInterestPos))
                    {
                        // opening tag s
                        if (openings <= 0)
                        {
                            startClosePos = xaml.IndexOf(">", tagOfInterestPos, StringComparison.Ordinal);
                            openings = 1;
                        }
                        else
                        {
                            openings += 1;
                        }
                    }
                }

                // Find next opening or closing tag
                var nextOpening = xaml.Substring(tagOfInterestPos + elementOpen.Length).FirstIndexOf(elementOpenComplete, elementOpenSpace);

                if (nextOpening > -1)
                {
                    nextOpening += tagOfInterestPos + elementOpen.Length;
                }

                var nextClosing = xaml.IndexOf(elementClose, tagOfInterestPos + elementOpen.Length, StringComparison.Ordinal);

                tagOfInterestPos = nextOpening > -1 && nextOpening < nextClosing ? nextOpening : nextClosing;
            }

            return exclusions;
        }

        public static string GetSubElementAtPosition(string xaml, int position)
        {
            var startPos = xaml.Substring(0, position).LastIndexOf('<');

            var elementName = xaml.Substring(startPos + 1, xaml.IndexOfAny(new[] { ' ', '>' }, startPos) - startPos - 1);

            string result = null;

            var processor = new SubElementProcessor();
            processor.SubElementFound += (s, e) => { result = e.SubElement; };

            XamlElementExtractor.Parse(null, xaml.Substring(startPos), new List<(string element, XamlElementProcessor processor)> { (elementName, processor), }, new List<IRapidXamlAdornmentTag>());

            return result;
        }

        // Use of snapshot in the Process implementation should be kept to a minimum as will need test workarounds
        // - better to just pass through to where needed in VS initiated functionality.
        public abstract void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags);

        public bool TryGetAttribute(string xaml, string attributeName, AttributeType attributeTypesToCheck, out AttributeType attributeType, out int index, out int length, out string value)
        {
            if (attributeTypesToCheck.HasFlag(AttributeType.Inline))
            {
                var searchText = $"{attributeName}=\"";

                var tbIndex = xaml.IndexOf(searchText, StringComparison.Ordinal);

                if (tbIndex >= 0)
                {
                    var tbEnd = xaml.IndexOf("\"", tbIndex + searchText.Length, StringComparison.Ordinal);

                    attributeType = AttributeType.Inline;
                    index = tbIndex;
                    length = tbEnd - tbIndex + 1;
                    value = xaml.Substring(tbIndex + searchText.Length, tbEnd - tbIndex - searchText.Length);
                    return true;
                }
            }

            var elementName = xaml.Substring(1, xaml.IndexOfAny(new[] { ' ', '>' }) - 1);

            if (attributeTypesToCheck.HasFlag(AttributeType.Element))
            {
                var searchText = $"<{elementName}.{attributeName}>";

                var startIndex = xaml.IndexOf(searchText, StringComparison.Ordinal);

                if (startIndex > -1)
                {
                    var closingElement = $"</{elementName}.{attributeName}>";
                    var endPos = xaml.IndexOf(closingElement, startIndex, StringComparison.Ordinal);

                    if (endPos > -1)
                    {
                        attributeType = AttributeType.Element;
                        index = startIndex;
                        length = endPos - startIndex + closingElement.Length;
                        value = xaml.Substring(startIndex + searchText.Length, endPos - startIndex - searchText.Length);
                        return true;
                    }
                }
            }

            if (attributeTypesToCheck.HasFlag(AttributeType.DefaultValue))
            {
                var endOfOpening = xaml.IndexOf(">");
                var closingTag = $"</{elementName}>";
                var startOfClosing = xaml.IndexOf(closingTag, StringComparison.Ordinal);

                if (startOfClosing > 0 && startOfClosing > endOfOpening)
                {
                    var defaultValue = xaml.Substring(endOfOpening + 1, startOfClosing - endOfOpening - 1);

                    if (!string.IsNullOrWhiteSpace(defaultValue) && !defaultValue.TrimStart().StartsWith("<"))
                    {
                        attributeType = AttributeType.DefaultValue;
                        index = 0;
                        length = xaml.Length;
                        value = defaultValue;
                        return true;
                    }
                }
            }

            attributeType = AttributeType.None;
            index = -1;
            length = 0;
            value = string.Empty;
            return false;
        }

        protected void CheckForHardCodedAttribute(string elementName, string attributeName, AttributeType types, string descriptionFormat, string xamlElement, ITextSnapshot snapshot, int offset, bool uidExists, string uidValue, Guid elementIdentifier, List<IRapidXamlAdornmentTag> tags)
        {
            if (this.TryGetAttribute(xamlElement, attributeName, types, out AttributeType foundAttributeType, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    tags.Add(new HardCodedStringTag(new Span(offset + tbIndex, length), snapshot, line.LineNumber, col, elementName, attributeName)
                    {
                        AttributeType = foundAttributeType,
                        Value = value,
                        Description = descriptionFormat.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uidValue,
                        ElementGuid = elementIdentifier,
                    });
                }
            }
        }

        protected (bool uidExists, string uidValue) GetOrGenerateUid(string xamlElement, string attributeName)
        {
            var uidExists = this.TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.Inline, out AttributeType _, out int _, out int _, out string uid);

            if (!uidExists)
            {
                // reuse `Name` or `x:Name` if exist
                if (this.TryGetAttribute(xamlElement, Attributes.Name, AttributeType.InlineOrElement, out AttributeType _, out int _, out int _, out string name))
                {
                    uid = name;
                }
                else
                {
                    this.TryGetAttribute(xamlElement, attributeName, AttributeType.InlineOrElement, out _, out _, out _, out string value);

                    var elementName = xamlElement.Substring(1, xamlElement.IndexOfAny(new[] { ' ', '>' }) - 1);

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        uid = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value)}{elementName}";

                        uid = uid.RemoveAllWhitespace().RemoveNonAlphaNumerics();
                    }
                    else
                    {
                        // This is just a large random number created to hopefully avoid collisions
                        uid = $"{elementName}{new Random().Next(1001, 8999)}";
                    }
                }
            }

            return (uidExists, uid);
        }

        public class SubElementProcessor : XamlElementProcessor
        {
            public event EventHandler<SubElementEventArgs> SubElementFound;

            public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
            {
                if (offset == 0)
                {
                    this.SubElementFound?.Invoke(this, new SubElementEventArgs { SubElement = xamlElement });
                }
            }

            public class SubElementEventArgs : EventArgs
            {
                public string SubElement { get; set; }
            }
        }
    }
}
