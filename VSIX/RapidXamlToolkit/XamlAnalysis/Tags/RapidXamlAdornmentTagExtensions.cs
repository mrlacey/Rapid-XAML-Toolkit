// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public static class RapidXamlAdornmentTagExtensions
    {
        public static int GetDesignerLineNumber(this RapidXamlAdornmentTag tag)
        {
            // ITextSnapshot uses a zero-indexed line count, while TextDocument (used in the designer) start the count at 1
            return tag.Snapshot.GetLineNumberFromPosition(tag.Span.Start) + 1;
        }
    }
}
