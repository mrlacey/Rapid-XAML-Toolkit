// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class InsertRowDefinitionTag : RapidXamlDiscreteTag
    {
        public InsertRowDefinitionTag((int Start, int Length) span, ITextSnapshotAbstraction snapshot, string fileName, ILogger logger)
            : base(span, snapshot, fileName, logger)
        {
        }

        // Used for text in suggested action ("Insert new row {RowId}")
        public int RowId { get; set; }

        public int RowCount { get; set; }

        public string XamlTag { get; set; }

        public int InsertPoint { get; set; }

        public int GridStartPos { get; set; }

        public int GridLength { get; set; }

        public Dictionary<int, int> ExclusionAreas { get; set; }
    }
}
