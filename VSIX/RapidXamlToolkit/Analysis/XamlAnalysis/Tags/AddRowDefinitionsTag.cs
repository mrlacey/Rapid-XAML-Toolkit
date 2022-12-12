// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddRowDefinitionsTag : InsertionTag
    {
        public AddRowDefinitionsTag((int Start, int Length) span, ITextSnapshotAbstraction snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, logger)
        {
        }
    }
}
