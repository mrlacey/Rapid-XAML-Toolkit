// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public static class XamlElementProcessorExtensions
    {
        public static void CheckForHardCodedAttribute(this XamlElementProcessor source, string fileName, string elementName, string attributeName, AttributeType types, string descriptionFormat, string xamlElement, ITextSnapshotAbstraction snapshot, int offset, bool uidExists, string uidValue, Guid elementIdentifier, TagList tags, List<TagSuppression> suppressions, ProjectType projType)
        {
            if (source.TryGetAttribute(xamlElement, attributeName, types, elementName, out AttributeType foundAttributeType, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var tagDeps = source.CreateBaseTagDependencies(
                        new VsTextSpan(offset + tbIndex, length),
                        snapshot,
                        fileName);

                    var tag = new HardCodedStringTag(tagDeps, elementName, attributeName, projType)
                    {
                        AttributeType = foundAttributeType,
                        Value = value,
                        Description = descriptionFormat.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uidValue,
                        ElementGuid = elementIdentifier,
                    };

                    tags.TryAdd(tag, xamlElement, suppressions);
                }
            }
        }

        public static void CheckForHardCodedAttribute(this XamlElementProcessor source, string fileName, string elementName, string attributeName, AttributeType types, string descriptionFormat, string xamlElement, ITextSnapshotAbstraction snapshot, int offset, string guidFallbackAttributeName, Guid elementIdentifier, TagList tags, List<TagSuppression> suppressions, ProjectType projType)
        {
            if (source.TryGetAttribute(xamlElement, attributeName, types, elementName, out AttributeType foundAttributeType, out int tbIndex, out int length, out string value))
            {
                if (!string.IsNullOrWhiteSpace(value) && char.IsLetterOrDigit(value[0]))
                {
                    var tagDeps = source.CreateBaseTagDependencies(
                        new VsTextSpan(offset + tbIndex, length),
                        snapshot,
                        fileName);

                    var (uidExists, uidValue) = source.GetOrGenerateUid(xamlElement, guidFallbackAttributeName);

                    var tag = new HardCodedStringTag(tagDeps, elementName, attributeName, projType)
                    {
                        AttributeType = foundAttributeType,
                        Value = value,
                        Description = descriptionFormat.WithParams(value),
                        UidExists = uidExists,
                        UidValue = uidValue,
                        ElementGuid = elementIdentifier,
                    };

                    tags.TryAdd(tag, xamlElement, suppressions);
                }
            }
        }

        public static (bool uidExists, string uidValue) GetOrGenerateUid(this XamlElementProcessor source, string xamlElement, string attributeName)
        {
            var uidExists = source.TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.Inline, out AttributeType _, out int _, out int _, out string uid);

            if (!uidExists)
            {
                // reuse `Name` or `x:Name` if exist
                if (source.TryGetAttribute(xamlElement, Attributes.Name, AttributeType.InlineOrElement, out AttributeType _, out int _, out int _, out string name))
                {
                    uid = name;
                }
                else
                {
                    var elementName = XamlElementProcessor.GetElementName(xamlElement.AsSpan());

                    if (source.TryGetAttribute(xamlElement, attributeName, AttributeType.InlineOrElement, out _, out _, out _, out string value))
                    {
                        uid = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value)}{elementName}";

                        uid = uid.RemoveAllWhitespace().RemoveNonAlphaNumerics();
                    }
                    else
                    {
                        // This is just a large random number created to hopefully avoid collisions
                        uid = $"{elementName}{new Random().Next(1001, 8999)}";
                    }
                }
            }

            return (uidExists, uid);
        }

        public static TagDependencies CreateBaseTagDependencies(this XamlElementProcessor source, ISpanAbstraction span, ITextSnapshotAbstraction snapshot, string fileName)
        {
            return new TagDependencies
            {
                Logger = source.Logger,
                VsPfp = source.VSPFP,
                ProjectFilePath = source.ProjectFilePath,
                Span = (span.Start, span.Length),
                Snapshot = snapshot,
                FileName = fileName,
            };
        }
    }
}
