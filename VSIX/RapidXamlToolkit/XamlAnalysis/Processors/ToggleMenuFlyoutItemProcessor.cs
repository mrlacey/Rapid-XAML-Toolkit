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
    public class ToggleMenuFlyoutItemProcessor : XamlElementProcessor
    {
        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Text);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.ToggleMenuFlyoutItem,
                Attributes.Text,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringToggleMenuFlyoutItemTextMessage,
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
