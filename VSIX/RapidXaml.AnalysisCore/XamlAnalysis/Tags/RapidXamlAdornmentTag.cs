// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlAdornmentTag : IRapidXamlAdornmentTag
    {
        public RapidXamlAdornmentTag((int Start, int Length) span, ITextSnapshotAbstraction snapshot, string fileName, ILogger logger)
        {
            this.Span = span;
            this.Snapshot = snapshot;
            this.FileName = fileName;
            this.Logger = logger;
        }

        public string ToolTip { get; set; }

        public (int Start, int Length) Span { get; set; }

        public ITextSnapshotAbstraction Snapshot { get; set; }

        public string FileName { get; }

        public ILogger Logger { get; }

        public string ErrorCode { get; set; }

        public TagErrorType ConfiguredErrorType => TagErrorType.Suggestion;
    }
}
