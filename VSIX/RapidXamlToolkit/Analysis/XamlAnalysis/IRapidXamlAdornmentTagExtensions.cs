// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class IRapidXamlAdornmentTagExtensions
    {
        public static ITagSpan<IErrorTag> AsErrorTag(this IRapidXamlAdornmentTag source)
        {
            var span = new SnapshotSpan((source.Snapshot as VsTextSnapshot).AsITextSnapshot(), new Span(source.Span.Start, source.Span.Length));

            if (source is RapidXamlDiscreteTag)
            {
                return new TagSpan<IErrorTag>(span, new RapidXamlSuggestionAdornmentTag(source.ToolTip));
            }
            else
            {
                return new TagSpan<IErrorTag>(
                    span,
                    new RapidXamlWarningAdornmentTag(
                        source.ToolTip,
                        source.ConfiguredErrorType.AsVsAdornmentErrorType()));
            }
        }
    }
}
