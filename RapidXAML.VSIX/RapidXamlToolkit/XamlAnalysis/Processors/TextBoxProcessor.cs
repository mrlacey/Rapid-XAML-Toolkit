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
