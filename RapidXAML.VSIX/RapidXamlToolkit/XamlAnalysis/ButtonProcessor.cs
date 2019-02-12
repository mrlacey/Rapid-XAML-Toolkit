// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class ButtonProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            const string searchText = "Content=\"";

            var tbIndex = xamlElement.IndexOf(searchText, StringComparison.Ordinal);

            if (tbIndex >= 0)
            {
                if (char.IsLetterOrDigit(xamlElement[tbIndex + searchText.Length]))
                {
                    var tbEnd = xamlElement.IndexOf("\"", tbIndex + searchText.Length, StringComparison.Ordinal);

                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    tags.Add(new OtherHardCodedStringTag(new Span(offset + tbIndex, tbEnd - tbIndex + 1), snapshot, line.LineNumber, col)
                    {
                        Description = "Buttons should not contain a hardcoded value for Content. Use a localized resource instead.",
                    });
                }
            }
        }
    }
}
