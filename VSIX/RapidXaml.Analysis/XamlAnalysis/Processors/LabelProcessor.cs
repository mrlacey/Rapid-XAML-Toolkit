// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    // This exists for testing development of functionality related to #163 & #410
    // Once they are implemented remove this in preference for LINK:CustomAnalysis/XamarinForms/LabelAnalyzer.cs
    public class LabelProcessor : XamlElementProcessor
    {
        public LabelProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xlmns = null)
        {
            if (!this.ProjectType.Matches(ProjectType.XamarinForms))
            {
                return;
            }

            //// var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Text);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.Label,
                Attributes.Text,
                AttributeType.Any,
                ////StringRes.UI_XamlAnalysisHardcodedStringTextblockTextMessage,
                "Label contains hard-coded Text value '{0}'",
                xamlElement,
                snapshot,
                offset,
                true,
                string.Empty,
                Guid.Empty,
                tags,
                suppressions,
                this.ProjectType);
        }
    }
}
