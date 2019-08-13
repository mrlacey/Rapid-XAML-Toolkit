// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBlockProcessor : XamlElementProcessor
    {
        public TextBlockProcessor(ProjectType projectType, ILogger logger)
            : base(projectType, logger)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Text);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.TextBlock,
                Attributes.Text,
                AttributeType.Any,
                StringRes.Info_XamlAnalysisHardcodedStringTextblockTextMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                Guid.Empty,
                tags,
                suppressions);
        }
    }
}
