// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBoxProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            // TODO: remove the duplication here and in TextBlockProcessor
            var uidExists = TryGetAttribute(xamlElement, Attributes.Uid, AttributeType.Inline, out AttributeType _, out int _, out int _, out string uid);

            if (!uidExists)
            {
                TryGetAttribute(xamlElement, Attributes.Header, AttributeType.Inline | AttributeType.Element, out _, out _, out _, out string headerValue);

                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    uid = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(headerValue)}TextBox";

                    // TODO: remove non-alphanumerics
                    uid = uid.RemoveAllWhitespace();
                }
                else
                {
                    // This is just a large random number created to hopefully avoid collisions
                    uid = $"TextBox{new Random().Next(10001, 99999)}";
                }
            }

            this.CheckForHardCodedAttribute(
                xamlElement,
                Attributes.Header,
                AttributeType.Inline | AttributeType.Element,
                snapshot,
                offset,
                uidExists,
                uid,
                StringRes.Info_XamlAnalysisHardcodedStringTextboxHeaderMessage,
                typeof(TextBoxHeaderAction),
                tags);

            this.CheckForHardCodedAttribute(
                xamlElement,
                Attributes.PlaceholderText,
                AttributeType.Inline | AttributeType.Element,
                snapshot,
                offset,
                uidExists,
                uid,
                StringRes.Info_XamlAnalysisHardcodedStringTextboxPlaceholderMessage,
                typeof(TextBoxPlaceholderAction),
                tags);
        }
    }
}
