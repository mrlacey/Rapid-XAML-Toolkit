// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class MissingDefinitionTag : RapidXamlDisplayedTag
    {
        protected MissingDefinitionTag(Span span, ITextSnapshot snapshot, string fileName, string errorCode, TagErrorType defaultErrorType, ILogger logger)
            : base(span, snapshot, fileName, errorCode, defaultErrorType, logger)
        {
        }

        public bool HasSomeDefinitions { get; set; }

        public int AssignedInt { get; set; }

        public int ExistingDefsCount { get; set; }

        public int TotalDefsRequired { get; set; }

        public int InsertPosition { get; set; }

        public string LeftPad { get; set; }
    }
}
