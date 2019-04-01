// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EntryProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            if (!this.TryGetAttribute(xamlElement, Attributes.Keyboard, AttributeType.Inline | AttributeType.Element, out _, out _, out _, out _))
            {
                var line = snapshot.GetLineFromPosition(offset);
                var col = offset - line.Start.Position;

                tags.Add(new AddEntryKeyboardTag(new Span(offset, xamlElement.Length), snapshot, line.LineNumber, col)
                {
                    InsertPosition = offset,
                });
            }
        }
    }
}
