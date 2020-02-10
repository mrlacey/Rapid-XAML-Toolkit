// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBoxProcessor : XamlElementProcessor
    {
        public TextBoxProcessor(ProjectType projectType, ILogger logger)
            : base(projectType, logger)
        {
        }

        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            if (!this.ProjectType.Matches(ProjectType.Uwp))
            {
                return;
            }

            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            var elementGuid = Guid.NewGuid();

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.TextBox,
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.UI_XamlAnalysisHardcodedStringTextboxHeaderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.TextBox,
                Attributes.PlaceholderText,
                AttributeType.Inline | AttributeType.Element,
                StringRes.UI_XamlAnalysisHardcodedStringTextboxPlaceholderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);

            if (!this.TryGetAttribute(xamlElement, Attributes.InputScope, AttributeType.Inline | AttributeType.Element, out _, out _, out _, out _))
            {
                tags.TryAdd(
                    new AddTextBoxInputScopeTag(new Span(offset, xamlElement.Length), snapshot, fileName, this.Logger)
                    {
                        InsertPosition = offset,
                    },
                    xamlElement,
                    suppressions);
            }
        }
    }
}
