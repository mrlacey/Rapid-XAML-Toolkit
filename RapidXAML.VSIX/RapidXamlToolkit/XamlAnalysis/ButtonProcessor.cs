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
            if (TryGetAttribute(xamlElement, "Content", out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + tbIndex);
                    var col = offset + tbIndex - line.Start.Position;

                    tags.Add(new OtherHardCodedStringTag(new Span(offset + tbIndex, length), snapshot, line.LineNumber, col)
                    {
                        Description = $"Button contains hardcoded Content value '{value}'.",
                    });
                }
            }
        }
    }
}
