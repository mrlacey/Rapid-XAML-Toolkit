﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public abstract class XamlElementProcessor
    {
        public XamlElementProcessor(ProcessorEssentials pe)
        {
            this.ProjectType = pe.ProjectType;
            this.Logger = pe.Logger;
            this.ProjectFilePath = pe.ProjectFilePath;
            this.VSPFP = pe.Vspfp;
        }

        public ProjectType ProjectType { get; }

        public ILogger Logger { get; }

        public string ProjectFilePath { get; }

        public IVisualStudioProjectFilePath VSPFP { get; }

        public static bool IsSelfClosing(ReadOnlySpan<char> xaml, int startPoint = 0)
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
            string elementOpenComplete = $"<{elementName}>";
            string elementClose = $"</{elementName}>";

            var exclusions = new Dictionary<int, int>();

            // Based on StringExtensions.FirstIndexOf
            int NextOpeningIndex(ReadOnlySpan<char> source)
            {
                var result = int.MaxValue;

                // Get complete opening tag
                var pos = source.Slice(0, source.Length).IndexOf(elementOpenComplete.AsSpan(), StringComparison.InvariantCultureIgnoreCase);

                if (pos > -1 && pos < result)
                {
                    result = pos;
                }

                // get opening followed by whitespace
                var keepLooking = true;
                var start = 0;

                while (keepLooking)
                {
                    pos = source.Slice(start, Math.Min(result, source.Length) - start).IndexOf(elementOpen.AsSpan(), StringComparison.InvariantCultureIgnoreCase);

                    if (pos > 0)
                    {
                        if (char.IsWhiteSpace(source[start + pos + elementOpen.Length]))
                        {
                            result = start + pos;
                            keepLooking = false;
                        }
                        else
                        {
                            start += pos + elementOpen.Length;
                        }
                    }
                    else
                    {
                        keepLooking = false;
                    }
                }

                return result < int.MaxValue ? result : -1;
            }

            // This is the opening position of the next opening (here) or closing (when set subsequently) tag
            var tagOfInterestPos = NextOpeningIndex(xaml.Substring(elementOpen.Length).AsSpan()) + elementOpen.Length;

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
                    if (!XamlElementProcessor.IsSelfClosing(xaml.AsSpan(), tagOfInterestPos))
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
                var nextOpening = NextOpeningIndex(xaml.Substring(tagOfInterestPos + elementOpen.Length).AsSpan());

                if (nextOpening > -1)
                {
                    nextOpening += tagOfInterestPos + elementOpen.Length;
                }

                var nextClosing = xaml.IndexOf(elementClose, tagOfInterestPos + elementOpen.Length, StringComparison.Ordinal);

                tagOfInterestPos = nextOpening > -1 && nextOpening < nextClosing ? nextOpening : nextClosing;
            }

            return exclusions;
        }

        public static string GetSubElementAtPosition(ProjectType projectType, string fileName, ITextSnapshotAbstraction snapshot, string xaml, int position, ILogger logger, string projectFile, IVisualStudioProjectFilePath vspfp)
        {
            var startPos = xaml.LastIndexOf('<', position, position);

            var elementName = GetElementName(xaml.AsSpan(), startPos);

            string result = null;

            var processor = new SubElementProcessor(new ProcessorEssentials(projectType, logger, projectFile, vspfp));
            processor.SubElementFound += (s, e) => { result = e.SubElement; };

            XamlElementExtractor.Parse(fileName, snapshot, xaml.Substring(startPos), new List<(string Element, XamlElementProcessor Processor)> { (elementName, processor), }, new TagList(), suppressions: null, everyElementProcessor: null, logger);

#if DEBUG
            if (result == null)
            {
                // If get here it's because there's a subelement that can't be identified correctly by XamlElementExtractor
                // but was detected elsewhere. (Probably by something extending XamlElementProcessor.)
                System.Diagnostics.Debugger.Break();
            }
#endif

            return result;
        }

        public static string GetElementName(ReadOnlySpan<char> xamlElement, int offset = 0)
        {
            return xamlElement.Slice(offset + 1, xamlElement.Slice(offset).IndexOfAny<char>(new[] { ' ', '>', '/', '\r', '\n' }) - 1).ToString();
        }

        public static string GetOpeningWithoutChildren(string xamlElementThatMayHaveChildren)
        {
            // Following logic assumes no whitespace at front
            xamlElementThatMayHaveChildren = xamlElementThatMayHaveChildren.TrimStart();

            if (xamlElementThatMayHaveChildren.EndsWith("/>"))
            {
                // If self-closing then definitely doesn't have children
                return xamlElementThatMayHaveChildren;
            }

            // Walk the string once rather than separate passes for each count
            var openTagCount = 0;
            var closeTagCount = 0;

            for (int i = 0; i < xamlElementThatMayHaveChildren.Length; i++)
            {
                var c = xamlElementThatMayHaveChildren[i];

                if (c == '<' && ++openTagCount > 2)
                {
                    break;
                }

                if (c == '>' && ++closeTagCount > 2)
                {
                    break;
                }
            }

            // Don't walk the whole string if we can avoid it for something without any sub-elements
            if (openTagCount == 2 && closeTagCount == 2)
            {
                return xamlElementThatMayHaveChildren.Substring(0, xamlElementThatMayHaveChildren.IndexOf('>') + 1).Trim();
            }

            int endOfElementName = 0;

            // Walk the string once rather than use LINQ
            for (int i = 0; i < xamlElementThatMayHaveChildren.Length; i++)
            {
                var c = xamlElementThatMayHaveChildren[i];

                if (c == ' ' || c == '\r' || c == '\n' || c == '>')
                {
                    endOfElementName = i;
                    break;
                }
            }

            var elementName = xamlElementThatMayHaveChildren.Substring(1, endOfElementName - 1);

            var nextElementStart = xamlElementThatMayHaveChildren.IndexOf('<', endOfElementName);

            while (nextElementStart > 0)
            {
                var possibleNextElementName = xamlElementThatMayHaveChildren.Substring(nextElementStart + 1, elementName.Length + 1);

                if (possibleNextElementName != $"{elementName}."
                    && possibleNextElementName != $"/{elementName}")
                {
                    var stringSoFar = xamlElementThatMayHaveChildren.Substring(0, nextElementStart);

                    int openings = 0;
                    int closings = 0;
                    int selfClosings = 0;

                    // Do all counting in a single pass of the string to avoid perf cost of multiple lookups
                    for (int i = 0; i < stringSoFar.Length - 1; i++)
                    {
                        if (stringSoFar[i] == '<')
                        {
                            if (stringSoFar[i + 1] == '/')
                            {
                                closings++;
                            }
                            else
                            {
                                openings++;
                            }
                        }
                        else if (stringSoFar[i] == '/' && stringSoFar[i + 1] == '>')
                        {
                            selfClosings++;
                        }
                    }

                    if (openings == closings + selfClosings + 1)
                    {
                        return stringSoFar;
                    }
                }

                nextElementStart = xamlElementThatMayHaveChildren.IndexOf('<', nextElementStart + 1);
            }

            // Only something unaccounted for above should get here - Give everything as fallback.
            return xamlElementThatMayHaveChildren;
        }

        /// <summary>
        /// Implementations of this method are used to identify any issues in the specified XAML and create Tags to highlight them.
        /// </summary>
        /// <remarks>
        /// Use of snapshot in the Process implementation should be kept to a minimum as it requires test workarounds
        /// - better to just pass through to where needed in VS initiated functionality.
        /// </remarks>
        /// <param name="fileName">The name of the file being analyzed.</param>
        /// <param name="offset">The number of characters from the start of the file to the element.</param>
        /// <param name="xamlElement">The full string representing the element to process.</param>
        /// <param name="linePadding">The amount of left padding the element has on the line where it starts.</param>
        /// <param name="snapshot">The ITextSnapshot containing the XAML being analyzed.</param>
        /// <param name="tags">Reference to the list of all tags found in the document. Add any new tags here.</param>
        /// <param name="suppressions">A list of user defined suppressions to override default behavior.</param>
        /// <param name="xmlns">A dictionalry of XML namespace aliases known by the document.</param>
        public abstract void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshotAbstraction snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xmlns = null);

        public bool TryGetAttribute(string xaml, string attributeName, AttributeType attributeTypesToCheck, out AttributeType attributeType, out int index, out int length, out string value)
        {
            // Pass null for elementName if don't already know it
            return this.TryGetAttribute(xaml, attributeName, attributeTypesToCheck, null, out attributeType, out index, out length, out value);
        }

        public bool TryGetAttribute(string xaml, string attributeName, AttributeType attributeTypesToCheck, string elementName, out AttributeType attributeType, out int index, out int length, out string value)
        {
            try
            {
                var xamlSpan = xaml.AsSpan();

                if (attributeTypesToCheck.HasFlag(AttributeType.Inline))
                {
                    if (string.IsNullOrWhiteSpace(xaml))
                    {
                        System.Diagnostics.Debugger.Break();
                        this.Logger?.RecordError($"xaml not passed to `TryGetAttribute({xaml}, {attributeName}, {attributeTypesToCheck})`");

                        attributeType = AttributeType.None;
                        index = -1;
                        length = 0;
                        value = string.Empty;
                        return false;
                    }

                    var searchText = $"{attributeName}=\"";

                    var tbIndex = xaml.IndexOf(searchText, StringComparison.Ordinal);

                    if (tbIndex >= 0 && char.IsWhiteSpace(xaml[tbIndex - 1]))
                    {
                        var tbEnd = xaml.IndexOf("\"", tbIndex + searchText.Length, StringComparison.Ordinal);

                        attributeType = AttributeType.Inline;
                        index = tbIndex;
                        length = tbEnd - tbIndex + 1;
                        value = xaml.Substring(tbIndex + searchText.Length, tbEnd - tbIndex - searchText.Length);
                        return true;
                    }
                }

                if (string.IsNullOrWhiteSpace(elementName))
                {
                    elementName = GetElementName(xamlSpan);
                }

                if (attributeTypesToCheck.HasFlag(AttributeType.Element))
                {
                    var searchText = $"<{elementName}.{attributeName}>";

                    var startIndex = xamlSpan.IndexOf(searchText.AsSpan(), StringComparison.Ordinal);

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
                    var endOfOpening = xamlSpan.IndexOf(">".AsSpan(), StringComparison.Ordinal);
                    var startOfClosing = xamlSpan.IndexOf($"</{elementName}>".AsSpan(), StringComparison.Ordinal);

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
            }
            catch (Exception exc)
            {
                this?.Logger?.RecordException(exc);
            }

            attributeType = AttributeType.None;
            index = -1;
            length = 0;
            value = string.Empty;
            return false;
        }

        public class SubElementProcessor : XamlElementProcessor
        {
            public SubElementProcessor(ProcessorEssentials essentials)
                : base(essentials)
            {
            }

            public event EventHandler<SubElementEventArgs> SubElementFound;

            public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshotAbstraction snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xlmns = null)
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
