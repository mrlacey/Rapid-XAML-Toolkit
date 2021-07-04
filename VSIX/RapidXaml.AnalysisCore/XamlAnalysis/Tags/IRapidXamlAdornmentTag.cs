// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlAdornmentTag : IRapidXamlTag
    {
        string ToolTip { get; set; }

        ITextSnapshotAbstraction Snapshot { get; set; }

        TagErrorType ConfiguredErrorType { get; }
    }
}
