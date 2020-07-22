// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public abstract class XamlElementProcessor
    {
        public XamlElementProcessor(ProcessorEssentials pe)
        {
            this.ProjectType = pe.ProjectType;
            this.Logger = pe.Logger;
            this.ProjectFilePath = pe.ProjectFilePath;
            this.VSAbstraction = pe.Vsa;
        }

        internal ProjectType ProjectType { get; }

        internal ILogger Logger { get; }

        internal string ProjectFilePath { get; }

        internal IVisualStudioAbstraction VSAbstraction { get; }

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

        public static string GetSubElementAtPosition(ProjectType projectType, string fileName, ITextSnapshot snapshot, string xaml, int position, ILogger logger, string projectFile, IVisualStudioAbstraction vsAbstraction)
        {
            var startPos = xaml.Substring(0, position).LastIndexOf('<');

            var elementName = GetElementName(xaml, startPos);

            string result = null;

            var processor = new SubElementProcessor(new ProcessorEssentials(projectType, logger, projectFile, vsAbstraction));
            processor.SubElementFound += (s, e) => { result = e.SubElement; };

            XamlElementExtractor.Parse(projectType, fileName, snapshot, xaml.Substring(startPos), new List<(string element, XamlElementProcessor processor)> { (elementName, processor), }, new TagList(), vsAbstraction);

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

        public static string GetElementName(string xamlElement, int offset = 0)
        {
            return xamlElement.Substring(offset + 1, xamlElement.IndexOfAny(new[] { ' ', '>', '/', '\r', '\n' }, offset) - offset - 1);
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

            // Don't walk the whole string if we can avoid it for something without any sub-elements
            if (xamlElementThatMayHaveChildren.Count(x => x == '<') == 2
             && xamlElementThatMayHaveChildren.Count(x => x == '>') == 2)
            {
                return xamlElementThatMayHaveChildren.Substring(0, xamlElementThatMayHaveChildren.IndexOf('>') + 1).Trim();
            }

            var endOfElementName = xamlElementThatMayHaveChildren.FirstIndexOf(" ", "\r", "\n", ">");

            var elementName = xamlElementThatMayHaveChildren.Substring(1, endOfElementName - 1);

            var nextElementStart = xamlElementThatMayHaveChildren.IndexOf('<', endOfElementName);

            var possibleNextElementName = string.Empty;

            while (nextElementStart > 0)
            {
                possibleNextElementName = xamlElementThatMayHaveChildren.Substring(nextElementStart + 1, elementName.Length + 1);

                if (possibleNextElementName != $"{elementName}."
                    && possibleNextElementName != $"/{elementName}")
                {
                    var stringSoFar = xamlElementThatMayHaveChildren.Substring(0, nextElementStart);

                    var openings = stringSoFar.Count(s => s == '<');
                    var closings = stringSoFar.OccurrenceCount("</");
                    openings -= closings; // Allow for the openings count incorrectly including closings
                    var selfClosings = stringSoFar.OccurrenceCount("/>");

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
        public abstract void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null);

        public bool TryGetAttribute(string xaml, string attributeName, AttributeType attributeTypesToCheck, out AttributeType attributeType, out int index, out int length, out string value)
        {
            try
            {
                if (attributeTypesToCheck.HasFlag(AttributeType.Inline))
                {
                    if (string.IsNullOrWhiteSpace(xaml))
                    {
                        System.Diagnostics.Debugger.Break();
                        this.Logger.RecordError($"xaml not passed to `TryGetAttribute({xaml}, {attributeName}, {attributeTypesToCheck})`");

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

                var elementName = GetElementName(xaml);

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

        protected void CheckForHardCodedAttribute(string fileName, string elementName, string attributeName, AttributeType types, string descriptionFormat, string xamlElement, ITextSnapshot snapshot, int offset, bool uidExists, string uidValue, Guid elementIdentifier, TagList tags, List<TagSuppression> suppressions, ProjectType projType)
        {
            if (this.TryGetAttribute(xamlElement, attributeName, types, out AttributeType foundAttributeType, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var tagDeps = this.CreateBaseTagDependencies(
                        new Span(offset + tbIndex, length),
                        snapshot,
                        fileName);

                    var tag = new HardCodedStringTag(tagDeps, elementName, attributeName, projType)
                    {
                        AttributeType = foundAttributeType,
                        Value = value,
                        Description = descriptionFormat.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uidValue,
                        ElementGuid = elementIdentifier,
                    };

                    tags.TryAdd(tag, xamlElement, suppressions);
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

                    var elementName = GetElementName(xamlElement);

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

        protected TagDependencies CreateBaseTagDependencies(Span span, ITextSnapshot snapshot, string fileName)
        {
            return new TagDependencies
            {
                Logger = this.Logger,
                VsAbstraction = this.VSAbstraction,
                ProjectFilePath = this.ProjectFilePath,
                Span = span,
                Snapshot = snapshot,
                FileName = fileName,
            };
        }

        public class SubElementProcessor : XamlElementProcessor
        {
            public SubElementProcessor(ProcessorEssentials essentials)
                : base(essentials)
            {
            }

            public event EventHandler<SubElementEventArgs> SubElementFound;

            public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
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
