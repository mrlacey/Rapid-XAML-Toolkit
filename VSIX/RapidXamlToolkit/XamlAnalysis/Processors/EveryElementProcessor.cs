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
    public class EveryElementProcessor : XamlElementProcessor
    {
        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (this.TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.InlineOrElement, out _, out int index, out int length, out string value))
            {
                if (!char.IsUpper(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + index);
                    var col = offset + index - line.Start.Position;
                    tags.TryAdd(new UidTitleCaseTag(new Span(offset + index, length), snapshot, fileName, line.LineNumber, col, value), xamlElement, suppressions);
                }
            }

            if (this.TryGetAttribute(xamlElement, Attributes.Name, AttributeType.InlineOrElement, out _, out index, out length, out value))
            {
                if (!char.IsUpper(value[0]))
                {
                    var line = snapshot.GetLineFromPosition(offset + index);
                    var col = offset + index - line.Start.Position;
                    tags.TryAdd(new NameTitleCaseTag(new Span(offset + index, length), snapshot, fileName, line.LineNumber, col, value), xamlElement, suppressions);
                }
            }
        }
    }
}
