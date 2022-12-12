// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class MissingDefinitionTag : RapidXamlDisplayedTag
    {
        protected MissingDefinitionTag(TagDependencies tagDeps, string errorCode, TagErrorType defaultErrorType)
            : base(tagDeps, errorCode, defaultErrorType)
        {
        }

        public bool HasSomeDefinitions { get; set; }

        public bool UsesShortDefinitionSyntax { get; set; }

        public int AssignedInt { get; set; }

        public int ExistingDefsCount { get; set; }

        public int TotalDefsRequired { get; set; }

        public int InsertPosition { get; set; }

        public string LeftPad { get; set; }
    }
}
