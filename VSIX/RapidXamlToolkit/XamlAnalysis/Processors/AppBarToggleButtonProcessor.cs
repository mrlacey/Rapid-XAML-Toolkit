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
    public class AppBarToggleButtonProcessor : XamlElementProcessor
    {
        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Label);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.AppBarToggleButton,
                Attributes.Label,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringAppBarToggleButtonLabelMessage,
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
