// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class AutoSuggestBoxProcessor : XamlElementProcessor
    {
        public AutoSuggestBoxProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshotAbstraction snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xlmns = null)
        {
            if (!this.ProjectType.Matches(ProjectType.Uwp))
            {
                return;
            }

            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            var elementGuid = Guid.NewGuid();

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.AutoSuggestBox,
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.UI_XamlAnalysisHardcodedStringAutoSuggestBoxHeaderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions,
                this.ProjectType);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.AutoSuggestBox,
                Attributes.PlaceholderText,
                AttributeType.InlineOrElement,
                StringRes.UI_XamlAnalysisHardcodedStringAutoSuggestBoxPlaceHolderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions,
                this.ProjectType);
        }
    }
}
