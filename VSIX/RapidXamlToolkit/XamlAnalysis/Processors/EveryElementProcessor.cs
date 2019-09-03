// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EveryElementProcessor : XamlElementProcessor
    {
        public EveryElementProcessor(ProjectType projectType, ILogger logger)
            : base(projectType, logger)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (this.TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.InlineOrElement, out _, out int index, out int length, out string value))
            {
                if (!char.IsUpper(value[0]))
                {
                    tags.TryAdd(new UidTitleCaseTag(new Span(offset + index, length), snapshot, fileName, value), xamlElement, suppressions);
                }
            }
            else if (this.TryGetAttribute(xamlElement, Attributes.X_Uid, AttributeType.InlineOrElement, out _, out index, out length, out value))
            {
                if (!char.IsUpper(value[0]))
                {
                    tags.TryAdd(new UidTitleCaseTag(new Span(offset + index, length), snapshot, fileName, value), xamlElement, suppressions);
                }
            }

            if (this.TryGetAttribute(xamlElement, Attributes.Name, AttributeType.InlineOrElement, out _, out index, out length, out value))
            {
                if (!char.IsUpper(value[0]))
                {
                    tags.TryAdd(new NameTitleCaseTag(new Span(offset + index, length), snapshot, fileName, value), xamlElement, suppressions);
                }
            }
            else if (this.TryGetAttribute(xamlElement, Attributes.X_Name, AttributeType.InlineOrElement, out _, out index, out length, out value))
            {
                if (!char.IsUpper(value[0]))
                {
                    tags.TryAdd(new NameTitleCaseTag(new Span(offset + index, length), snapshot, fileName, value), xamlElement, suppressions);
                }
            }
        }
    }
}
