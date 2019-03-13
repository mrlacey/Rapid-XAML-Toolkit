// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class MenuFlyoutItemProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Text);

            this.CheckForHardCodedAttribute(
                Attributes.Text,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringMenuFlyoutItemTextMessage,
                typeof(MenuFlyoutItemTextAction),
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                tags);
        }
    }
}
