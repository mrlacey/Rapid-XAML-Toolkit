// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class ToggleSwitchProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            this.CheckForHardCodedAttribute(
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringToggleSwitchHeaderMessage,
                typeof(ToggleSwithHeaderAction),
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);

            this.CheckForHardCodedAttribute(
                Attributes.OnContent,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringToggleSwitchOnContentMessage,
                typeof(ToggleSwithOnContentAction),
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);

            this.CheckForHardCodedAttribute(
                Attributes.OffContent,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringToggleSwitchOffContentMessage,
                typeof(ToggleSwithOffContentAction),
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);
        }
    }
}
