// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.ErrorList
{
    public class ErrorRow
    {
        public string ExtendedMessage { get; set; }

        public string Message { get; set; }

        public SnapshotSpan Span { get; set; }

        public string ErrorCode { get; internal set; }

        public bool IsInternalError { get; internal set; }

        public TagErrorType ErrorType { get; internal set; }

        public string MoreInfoUrl { get; internal set; }
    }
}
