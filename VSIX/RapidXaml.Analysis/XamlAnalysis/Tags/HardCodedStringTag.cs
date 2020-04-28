// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class HardCodedStringTag : RapidXamlDisplayedTag
    {
        public HardCodedStringTag(TagDependencies tagDeps, string elementName, string attributeName, ProjectType projType)
            : base(tagDeps, "RXT200", TagErrorType.Warning)
        {
            this.SuggestedAction = typeof(HardCodedStringAction);
            this.ToolTip = StringRes.UI_XamlAnalysisHardcodedStringTooltip;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisHardcodedStringExtendedMessage;
            this.ElementName = elementName;
            this.AttributeName = attributeName;
            this.ProjType = projType;
        }

        public AttributeType AttributeType { get; set; }

        public string Value { get; set; }

        public bool UidExists { get; set; }

        public string UidValue { get; set; }

        public string ElementName { get; }

        public string AttributeName { get; }

        public Guid ElementGuid { get; set; }

        public ProjectType ProjType { get; set; }
    }
}
