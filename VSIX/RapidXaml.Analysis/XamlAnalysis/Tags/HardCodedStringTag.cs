// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class HardCodedStringTag : RapidXamlDisplayedTag
    {
        public HardCodedStringTag(Span span, ITextSnapshot snapshot, string fileName, string elementName, string attributeName, ILogger logger)
            : base(span, snapshot, fileName, "RXT200", TagErrorType.Warning, logger)
        {
            this.SuggestedAction = typeof(HardCodedStringAction);
            this.ToolTip = StringRes.UI_XamlAnalysisHardcodedStringTooltip;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisHardcodedStringExtendedMessage;
            this.ElementName = elementName;
            this.AttributeName = attributeName;
        }

        public AttributeType AttributeType { get; set; }

        public string Value { get; set; }

        public bool UidExists { get; set; }

        public string UidValue { get; set; }

        public string ElementName { get; }

        public string AttributeName { get; }

        public Guid ElementGuid { get; set; }
    }
}
