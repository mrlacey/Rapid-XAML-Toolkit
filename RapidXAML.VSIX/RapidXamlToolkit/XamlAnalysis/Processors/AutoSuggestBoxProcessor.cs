// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class AutoSuggestBoxProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            this.CheckForHardCodedAttribute(
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringAutoSuggestBoxHeaderMessage,
                typeof(AutoSuggestBoxHeaderAction),
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);

            this.CheckForHardCodedAttribute(
                Attributes.PlaceholderText,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringAutoSuggestBoxPlaceHolderMessage,
                typeof(AutoSuggestBoxHeaderAction),
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);
        }
    }
}
