// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class InsertRowDefinitionTag : IRapidXamlTag
    {
        public ActionTypes ActionType => ActionTypes.InsertRowDefinition;

        // Used for text in suggested action ("Insert new row {RowId}")
        public int RowId { get; set; }

        public int RowCount { get; set; }

        public Span Span { get; set; }

        public string XamlTag { get; set; }

        public int InsertPoint { get; set; }

        public int GridStartPos { get; set; }

        public int GridLength { get; set; }

        public Dictionary<int, int> ExclusionAreas { get; set; }
    }
}
