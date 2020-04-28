// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EntryProcessor : XamlElementProcessor
    {
        public EntryProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (!this.ProjectType.Matches(ProjectType.XamarinForms))
            {
                return;
            }

            if (!this.TryGetAttribute(xamlElement, Attributes.Keyboard, AttributeType.Inline | AttributeType.Element, out _, out _, out _, out _))
            {
                var tagDeps = this.CreateBaseTagDependencies(
                    new Span(offset, xamlElement.Length),
                    snapshot,
                    fileName);

                tags.TryAdd(
                    new AddEntryKeyboardTag(tagDeps, xamlElement)
                    {
                        InsertPosition = offset,
                    },
                    xamlElement,
                    suppressions);
            }
        }
    }
}
