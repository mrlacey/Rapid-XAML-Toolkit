// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class InsertRowDefinitionTag : IRapidXamlTag
    {
        public ActionTypes ActionType => ActionTypes.InsertRowDefinition;

        // Used for text in suggested action ("Insert new row {RowId}")
        public int RowId { get; set; }

        public Span Span { get; set; }
    }
}
