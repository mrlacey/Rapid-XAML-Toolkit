// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EntryProcessor : XamlElementProcessor
    {
        public EntryProcessor(ProjectType projectType, ILogger logger)
            : base(projectType, logger)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (!this.TryGetAttribute(xamlElement, Attributes.Keyboard, AttributeType.Inline | AttributeType.Element, out _, out _, out _, out _))
            {
                var line = snapshot.GetLineFromPosition(offset);
                var col = offset - line.Start.Position;

                tags.TryAdd(
                    new AddEntryKeyboardTag(new Span(offset, xamlElement.Length), snapshot, fileName, line.LineNumber, col, xamlElement)
                    {
                        InsertPosition = offset,
                    },
                    xamlElement,
                    suppressions);
            }
        }
    }
}
