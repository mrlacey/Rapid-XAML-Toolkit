// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class MediaElementProcessor : XamlElementProcessor
    {
        public MediaElementProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, IRapidXamlTextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (!this.ProjectType.Matches(ProjectType.Uwp))
            {
                return;
            }

            var tagDeps = this.CreateBaseTagDependencies(
                new Span(offset, xamlElement.Length),
                snapshot,
                fileName);

            tags.TryAdd(
                new UseMediaPlayerElementTag(tagDeps)
                {
                    InsertPosition = offset,
                },
                xamlElement,
                suppressions);
        }
    }
}
