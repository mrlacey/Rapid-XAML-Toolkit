// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBlockProcessor : XamlElementProcessor
    {
        // TODO: need to allow for default value as well as being an attribute
        // TODO: allow for attribute value being set as a child element (e.g. <TextBlock><TextBlock.Text>Click here</TextBlock.Text></TextBlock>)
        // TODO: Need to let action deal with whether attribute/child-element-attribute/default-value
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            if (TryGetAttribute(xamlElement, Attributes.Text, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    var uidExists = TryGetAttribute(xamlElement, Attributes.Uid, out int _, out int _, out string uid);

                    if (!uidExists)
                    {
                        uid = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value)}TextBlock";
                    }

                    tags.Add(new HardCodedStringTag(new Span(offset + tbIndex, length), snapshot, line.LineNumber, col)
                    {
                        Value = value,
                        Description = StringRes.Info_XamlAnalysisHardcodedStringTextblockTextMessage.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uid,
                    });
                }
            }
        }
    }
}
