// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
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
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            this.CheckForHardCodedAttribute(
                Elements.TextBox,
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringTextboxHeaderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);

            this.CheckForHardCodedAttribute(
                Elements.TextBox,
                Attributes.PlaceholderText,
                AttributeType.Inline | AttributeType.Element,
                StringRes.Info_XamlAnalysisHardcodedStringTextboxPlaceholderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);

            if (!this.TryGetAttribute(xamlElement, Attributes.InputScope, AttributeType.Inline | AttributeType.Element, out _, out _, out _, out _))
            {
                var line = snapshot.GetLineFromPosition(offset);
                var col = offset - line.Start.Position;

                tags.Add(new AddTextBoxInputScopeTag(new Span(offset, xamlElement.Length), snapshot, line.LineNumber, col)
                {
                    InsertPosition = offset,
                });
            }
        }
    }
}
