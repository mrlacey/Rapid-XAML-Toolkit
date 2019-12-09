// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class ToggleSwitchProcessor : XamlElementProcessor
    {
        public ToggleSwitchProcessor(ProjectType projectType, ILogger logger)
            : base(projectType, logger)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (!this.ProjectType.Matches(ProjectType.Uwp))
            {
                return;
            }

            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            var elementGuid = Guid.NewGuid();

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.ToggleSwitch,
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringToggleSwitchHeaderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.ToggleSwitch,
                Attributes.OnContent,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringToggleSwitchOnContentMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.ToggleSwitch,
                Attributes.OffContent,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringToggleSwitchOffContentMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);
        }
    }
}
