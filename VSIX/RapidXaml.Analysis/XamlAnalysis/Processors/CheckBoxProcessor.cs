// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class CheckBoxProcessor : XamlElementProcessor
    {
        public CheckBoxProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (this.ProjectType.Matches(ProjectType.Uwp) || this.ProjectType.Matches(ProjectType.Wpf))
            {
                if (this.ProjectType.Matches(ProjectType.Uwp))
                {
                    var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Content);

                    this.CheckForHardCodedAttribute(
                        fileName,
                        Elements.CheckBox,
                        Attributes.Content,
                        AttributeType.Any,
                        StringRes.UI_XamlAnalysisHardcodedStringCheckboxContentMessage,
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

                // If using one event, the recommendation is to use both
                var hasCheckedEvent = this.TryGetAttribute(xamlElement, Attributes.CheckedEvent, AttributeType.Inline, out _, out int checkedIndex, out int checkedLength, out string checkedEventName);
                var hasuncheckedEvent = this.TryGetAttribute(xamlElement, Attributes.UncheckedEvent, AttributeType.Inline, out _, out int uncheckedIndex, out int uncheckedLength, out string uncheckedEventName);

                if (hasCheckedEvent && !hasuncheckedEvent)
                {
                    var checkedTag = new CheckBoxCheckedAndUncheckedEventsTag(new Span(offset + checkedIndex, checkedLength), snapshot, fileName, checkedEventName, hasChecked: true, this.Logger, this.VSAbstraction, this.ProjectFile)
                    {
                        InsertPosition = offset,
                    };

                    tags.TryAdd(checkedTag, xamlElement, suppressions);
                }

                if (!hasCheckedEvent && hasuncheckedEvent)
                {
                    var uncheckedTag = new CheckBoxCheckedAndUncheckedEventsTag(new Span(offset + uncheckedIndex, uncheckedLength), snapshot, fileName, uncheckedEventName, hasChecked: false, this.Logger, this.VSAbstraction, this.ProjectFile)
                    {
                        InsertPosition = offset,
                    };

                    tags.TryAdd(uncheckedTag, xamlElement, suppressions);
                }
            }
        }
    }
}
