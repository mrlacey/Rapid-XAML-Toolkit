// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class ButtonProcessor : XamlElementProcessor
    {
        // TODO: need to allow for default value as well as being an attribute
        // TODO: allow for attribute value being set as a child element (e.g. <Button><Button.Content>Click here</Button.Content></Button>)
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            if (TryGetAttribute(xamlElement, Attributes.Content, AttributeType.Any, out AttributeType foundAttributeType, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    tags.Add(new OtherHardCodedStringTag(new Span(offset + tbIndex, length), snapshot, line.LineNumber, col)
                    {
                        Description = StringRes.Info_XamlAnalysisHardcodedStringButtonContentMessage.WithParams(value),
                    });
                }
            }
        }
    }
}
