// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class MediaElementProcessor : XamlElementProcessor
    {
        public MediaElementProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshotAbstraction snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xlmns = null)
        {
            if (!this.ProjectType.Matches(ProjectType.Uwp))
            {
                return;
            }

            var tagDeps = this.CreateBaseTagDependencies(
                new VsTextSpan(offset, xamlElement.Length),
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
