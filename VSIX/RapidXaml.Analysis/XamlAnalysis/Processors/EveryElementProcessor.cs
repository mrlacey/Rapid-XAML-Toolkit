// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EveryElementProcessor : XamlElementProcessor
    {
        public EveryElementProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xlmns = null)
        {
            // TODO: need to determine position of end of opening when parsing original XAML to avoid this lookup each time
            // Remove children to avoid getting duplicates when children are processed.
            xamlElement = GetOpeningWithoutChildren(xamlElement);

            if (!xamlElement.Contains("=") && !xamlElement.Contains("."))
            {
                // There are no attributes so skip any further checks
                return;
            }

            if (this.TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.InlineOrElement, out _, out int index, out int length, out string value))
            {
                if (!char.IsUpper(value[0]))
                {
                    var tagDeps = this.CreateBaseTagDependencies(
                        new Span(offset + index, length),
                        snapshot,
                        fileName);

                    tags.TryAdd(new UidTitleCaseTag(tagDeps, value), xamlElement, suppressions);
                }
            }
            else if (this.TryGetAttribute(xamlElement, Attributes.X_Uid, AttributeType.InlineOrElement, out _, out index, out length, out value))
            {
                if (!char.IsUpper(value[0]))
                {
                    var tagDeps = this.CreateBaseTagDependencies(
                        new Span(offset + index, length),
                        snapshot,
                        fileName);

                    tags.TryAdd(new UidTitleCaseTag(tagDeps, value), xamlElement, suppressions);
                }
            }

            if (this.TryGetAttribute(xamlElement, Attributes.Name, AttributeType.InlineOrElement, out _, out index, out length, out value))
            {
                if (!char.IsUpper(value[0]))
                {
                    var tagDeps = this.CreateBaseTagDependencies(
                        new Span(offset + index, length),
                        snapshot,
                        fileName);

                    tags.TryAdd(new NameTitleCaseTag(tagDeps, value), xamlElement, suppressions);
                }
            }
            else if (this.TryGetAttribute(xamlElement, Attributes.X_Name, AttributeType.InlineOrElement, out _, out index, out length, out value))
            {
                if (!char.IsUpper(value[0]))
                {
                    var tagDeps = this.CreateBaseTagDependencies(
                        new Span(offset + index, length),
                        snapshot,
                        fileName);

                    tags.TryAdd(new NameTitleCaseTag(tagDeps, value), xamlElement, suppressions);
                }
            }

            if (this.ProjectType.Matches(ProjectType.Uwp))
            {
                var nameEndPos = xamlElement.IndexOfAny(new[] { ' ', '/', '>' });
                var elementName = xamlElement.Substring(1, nameEndPos - 1);

                this.CheckForHardCodedAttribute(
                    fileName,
                    elementName,
                    Attributes.TooltipServiceDotToolTip,
                    AttributeType.Inline,
                    StringRes.UI_XamlAnalysisHardcodedStringTooltipServiceToolTipMessage,
                    xamlElement,
                    snapshot,
                    offset,
                    Attributes.Header,
                    Guid.Empty,
                    tags,
                    suppressions,
                    this.ProjectType);
            }
        }
    }
}
