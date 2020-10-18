// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.Win32.SafeHandles;
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
                Span = new SnapshotSpan((ITextSnapshot)source.Snapshot, new Span(source.Span.Start, source.Span.Length)),
                Message = source.Description,
                ErrorCode = source.ErrorCode,
                IsInternalError = source.IsInternalError,
                ErrorType = source.ConfiguredErrorType,
                MoreInfoUrl = source.MoreInfoUrl,
            };
        }
    }
}
