// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public abstract class XamlElementProcessor
    {
        // Use of snapshot in the Process implementation should be kept to a minimum as will need test workarounds
        // - better to just pass through to where needed in VS initiated functionality.
        public abstract void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags);

        // This should wrap XAH.HasAttribute & GetAttribute - and/or more there
        protected static bool TryGetAttribute(string xaml, string attributeName, AttributeType attributeTypesToCheck, out AttributeType attributeType, out int index, out int length, out string value)
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

                    if (!string.IsNullOrWhiteSpace(defaultValue))
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

        protected void CheckForHardCodedAttribute(string xamlElement, string attributeName, AttributeType types, ITextSnapshot snapshot, int offset, bool uidExists, string uidValue, string descriptionFormat, Type action, List<IRapidXamlAdornmentTag> tags)
        {
            if (TryGetAttribute(xamlElement, attributeName, types, out AttributeType foundAttributeType, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    tags.Add(new HardCodedStringTag(new Span(offset + tbIndex, length), snapshot, line.LineNumber, col, action)
                    {
                        AttributeType = foundAttributeType,
                        Value = value,
                        Description = descriptionFormat.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uidValue,
                    });
                }
            }
        }
    }
}
