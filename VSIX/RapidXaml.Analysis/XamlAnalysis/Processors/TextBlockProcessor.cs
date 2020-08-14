// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBlockProcessor : XamlElementProcessor
    {
        public TextBlockProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, IRapidXamlTextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (!this.ProjectType.Matches(ProjectType.Uwp))
            {
                return;
            }

            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Text);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.TextBlock,
                Attributes.Text,
                AttributeType.Any,
                StringRes.UI_XamlAnalysisHardcodedStringTextblockTextMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                Guid.Empty,
                tags,
                suppressions,
                this.ProjectType);
        }
    }
}
