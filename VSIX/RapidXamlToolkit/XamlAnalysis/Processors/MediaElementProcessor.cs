// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class MediaElementProcessor : XamlElementProcessor
    {
        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            var line = snapshot.GetLineFromPosition(offset);
            var col = offset - line.Start.Position;
            tags.Add(new UseMediaPlayerElementTag(new Span(offset, xamlElement.Length), snapshot, fileName, line.LineNumber, col)
            {
                InsertPosition = offset,
            });
        }
    }
}
