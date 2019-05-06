// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class SelectedItemAttributeProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            if (this.TryGetAttribute(xamlElement, Attributes.SelectedItem, AttributeType.Inline | AttributeType.Element, out _, out int index, out int length, out string value))
            {
                if (value.StartsWith("{") && !value.Contains("TwoWay"))
                {
                    var line = snapshot.GetLineFromPosition(offset);

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

                    tags.Add(new SelectedItemModeTag(new Span(offset + index, length), snapshot, line.LineNumber, index)
                    {
                        InsertPosition = offset + index,
                        ExistingBindingMode = existingMode,
                    });
                }
            }
        }
    }
}
