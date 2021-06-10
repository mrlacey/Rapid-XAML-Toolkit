// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class RapidXamlDisplayedTagExtensions
    {
        public static ErrorRow AsErrorRow(this RapidXamlDisplayedTag source)
        {
            return new ErrorRow
            {
                ExtendedMessage = source.ExtendedMessage,
                Span = new SnapshotSpan((source.Snapshot as VsTextSnapshot).AsITextSnapshot(), new Span(source.Span.Start, source.Span.Length)),
                Message = source.Description,
                ErrorCode = source.ErrorCode,
                IsInternalError = source.IsInternalError,
                ErrorType = source.ConfiguredErrorType,
                MoreInfoUrl = source.MoreInfoUrl,
            };
        }
    }
}
