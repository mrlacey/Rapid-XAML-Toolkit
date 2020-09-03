// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class InsertRowDefinitionTag : RapidXamlDiscreteTag
    {
        public InsertRowDefinitionTag(Span span, ITextSnapshot snapshot, string fileName, ILogger logger)
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
