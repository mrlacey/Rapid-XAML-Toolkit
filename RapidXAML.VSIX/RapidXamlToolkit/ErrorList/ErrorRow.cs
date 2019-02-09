// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.ErrorList
{
    public class ErrorRow
    {
        // TODO: need to pass this in
        public string ExtendedMessage { get; set; }

        public string Message { get; set; }

        public SnapshotSpan Span { get; set; }

        public string ErrorCode { get; internal set; }

        public bool IsFatal { get; internal set; }
    }
}
