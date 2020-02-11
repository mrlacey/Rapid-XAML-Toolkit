// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class LabelProcessor : XamlElementProcessor
    {
        public LabelProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (!this.ProjectType.Matches(ProjectType.XamarinForms))
            {
                return;
            }

            // TODO: ISSUE#163 reinstate this when can handle localization of Xamarin.Forms apps
            ////var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            ////this.CheckForHardCodedAttribute(
            ////    fileName,
            ////    Elements.Label,
            ////    Attributes.Text,
            ////    AttributeType.InlineOrElement,
            ////    StringRes.Info_XamlAnalysisHardcodedStringLabelTextMessage,
            ////    xamlElement,
            ////    snapshot,
            ////    offset,
            ////    uidExists,
            ////    uid,
            ////    Guid.Empty,
            ////    tags,
            ////    suppressions);
        }
    }
}
