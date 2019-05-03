// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class MissingDefinitionTag : RapidXamlDisplayedTag
    {
        protected MissingDefinitionTag(Span span, ITextSnapshot snapshot, string errorCode, int line, int column, TagErrorType defaultErrorType)
            : base(span, snapshot, errorCode, line, column, defaultErrorType)
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
