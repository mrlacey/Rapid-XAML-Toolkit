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
    public class CheckBoxProcessor : XamlElementProcessor
    {
        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Content);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.CheckBox,
                Attributes.Content,
                AttributeType.Any,
                StringRes.Info_XamlAnalysisHardcodedStringCheckboxContentMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                Guid.Empty,
                tags);

            // If using one event, the recommendation is to use both
            var hasCheckedEvent = this.TryGetAttribute(xamlElement, Attributes.CheckedEvent, AttributeType.Inline, out _, out int checkedIndex, out int checkedLength, out string checkedEventName);
            var hasuncheckedEvent = this.TryGetAttribute(xamlElement, Attributes.UncheckedEvent, AttributeType.Inline, out _, out int uncheckedIndex, out int uncheckedLength, out string uncheckedEventName);

            if (hasCheckedEvent && !hasuncheckedEvent)
            {
                var line = snapshot.GetLineFromPosition(offset + checkedIndex);
                var col = offset + checkedIndex - line.Start.Position;
                tags.Add(new CheckBoxCheckedAndUncheckedEventsTag(new Span(offset + checkedIndex, checkedLength), snapshot, fileName, line.LineNumber, col, checkedEventName, hasChecked: true)
                {
                    InsertPosition = offset,
                });
            }

            if (!hasCheckedEvent && hasuncheckedEvent)
            {
                var line = snapshot.GetLineFromPosition(offset + uncheckedIndex);
                var col = offset + uncheckedIndex - line.Start.Position;
                tags.Add(new CheckBoxCheckedAndUncheckedEventsTag(new Span(offset + uncheckedIndex, uncheckedLength), snapshot, fileName, line.LineNumber, col, uncheckedEventName, hasChecked: false)
                {
                    InsertPosition = offset,
                });
            }
        }
    }
}
