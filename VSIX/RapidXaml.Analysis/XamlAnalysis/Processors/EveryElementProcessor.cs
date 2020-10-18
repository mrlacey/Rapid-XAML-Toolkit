// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EveryElementProcessor : XamlElementProcessor
    {
        public EveryElementProcessor(ProcessorEssentials essentials)
            : base(essentials)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshotAbstraction snapshot, TagList tags, List<TagSuppression> suppressions = null, Dictionary<string, string> xlmns = null)
        {
            if (!xamlElement.Contains("=") && !xamlElement.Contains("."))
            {
                // There are no attributes so skip any further checks
                return;
            }

            // This check is cheaper than two unnecessary calls to TryGetAttribute
            if (xamlElement.Contains($"{Attributes.Uid}="))
            {
                if (this.TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.InlineOrElement, out _, out int index, out int length, out string value))
                {
                    if (!char.IsUpper(value[0]))
                    {
                        var tagDeps = this.CreateBaseTagDependencies(
                            new VsTextSpan(offset + index, length),
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
                            new VsTextSpan(offset + index, length),
                            snapshot,
                            fileName);

                        tags.TryAdd(new UidTitleCaseTag(tagDeps, value), xamlElement, suppressions);
                    }
                }
            }

            // This check is cheaper than two unnecessary calls to TryGetAttribute
            if (xamlElement.Contains($"{Attributes.Name}="))
            {
                if (this.TryGetAttribute(xamlElement, Attributes.Name, AttributeType.InlineOrElement, out _, out int index, out int length, out string value))
                {
                    if (!char.IsUpper(value[0]))
                    {
                        var tagDeps = this.CreateBaseTagDependencies(
                            new VsTextSpan(offset + index, length),
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
                            new VsTextSpan(offset + index, length),
                            snapshot,
                            fileName);

                        tags.TryAdd(new NameTitleCaseTag(tagDeps, value), xamlElement, suppressions);
                    }
                }
            }

            // The Contains check is cheaper than getting the StringRes to pass to CheckForHardCodedAttribute, let alone running it
            if (this.ProjectType.Matches(ProjectType.Uwp)
             && xamlElement.Contains($"{Attributes.TooltipServiceDotToolTip}="))
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
