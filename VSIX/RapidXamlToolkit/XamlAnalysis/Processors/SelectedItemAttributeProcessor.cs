// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class SelectedItemAttributeProcessor : XamlElementProcessor
    {
        public SelectedItemAttributeProcessor(ProjectType projectType, ILogger logger)
            : base(projectType, logger)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (this.TryGetAttribute(xamlElement, Attributes.SelectedItem, AttributeType.Inline | AttributeType.Element, out _, out int index, out int length, out string value))
            {
                if (value.StartsWith("{") && !value.Contains("TwoWay"))
                {
                    string existingMode = null;

                    const string oneTime = "Mode=OneTime";
                    const string oneWay = "Mode=OneWay";

                    if (value.Contains(oneTime))
                    {
                        existingMode = oneTime;
                    }
                    else if (value.Contains(oneWay))
                    {
                        existingMode = oneWay;
                    }

                    tags.TryAdd(
                        new SelectedItemBindingModeTag(new Span(offset + index, length), snapshot, fileName)
                        {
                            InsertPosition = offset + index,
                            ExistingBindingMode = existingMode,
                        },
                        xamlElement,
                        suppressions);
                }
            }
        }
    }
}
