// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class TextBlockProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, ITextSnapshot snapshot, List<IRapidXamlTag> tags)
        {
            const string searchText = "Text=\"";

            var tbIndex = xamlElement.IndexOf(searchText, StringComparison.Ordinal);

            if (tbIndex >= 0)
            {
                if (char.IsLetterOrDigit(xamlElement[tbIndex + searchText.Length]))
                {
                    var tbEnd = xamlElement.IndexOf("\"", tbIndex + searchText.Length, StringComparison.Ordinal);

                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    tags.Add(new HardCodedStringTag
                    {
                        Span = new Span(offset + tbIndex, tbEnd - tbIndex + 1),
                        Line = line.LineNumber,
                        Column = col,
                        Snapshot = snapshot,
                        Message = "TextBlock should not contain a hardcoded value for Text. Use a localized resource instead.",
                    });
                }
            }
        }
    }
}
