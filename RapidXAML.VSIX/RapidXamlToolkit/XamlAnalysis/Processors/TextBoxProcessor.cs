// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBoxProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            // TODO: remove the duplication here and in TextBlockProcessor
            if (TryGetAttribute(xamlElement, Attributes.Header, AttributeType.Inline | AttributeType.Element, out AttributeType foundAttributeType, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    var uidExists = TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.Inline, out AttributeType _, out int _, out int _, out string uid);

                    if (!uidExists)
                    {
                        uid = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value)}TextBox";
                        uid = uid.RemoveAllWhitespace();
                    }

                    tags.Add(new HardCodedStringTag(new Span(offset + tbIndex, length), snapshot, line.LineNumber, col, typeof(TextBoxHeaderAction))
                    {
                        AttributeType = foundAttributeType,
                        Value = value,
                        Description = StringRes.Info_XamlAnalysisHardcodedStringTextboxHeaderMessage.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uid,
                    });
                }
            }

            if (TryGetAttribute(xamlElement, Attributes.PlaceholderText, AttributeType.Inline | AttributeType.Element, out foundAttributeType, out tbIndex, out length, out value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    var uidExists = TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.Inline, out AttributeType _, out int _, out int _, out string uid);

                    if (!uidExists)
                    {
                        uid = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value)}TextBox";
                        uid = uid.RemoveAllWhitespace();
                    }

                    tags.Add(new HardCodedStringTag(new Span(offset + tbIndex, length), snapshot, line.LineNumber, col, typeof(TextBoxPlaceholderAction))
                    {
                        AttributeType = foundAttributeType,
                        Value = value,
                        Description = StringRes.Info_XamlAnalysisHardcodedStringTextboxPlaceholderMessage.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uid,
                    });
                }
            }
        }
    }
}
