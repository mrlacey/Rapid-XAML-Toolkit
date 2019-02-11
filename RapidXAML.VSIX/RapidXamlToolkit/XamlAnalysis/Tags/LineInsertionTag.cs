// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class LineInsertionTag : IRapidXamlTag
    {
        public ActionTypes ActionType { get; }

        public Span Span { get; set; }

        public int InsertLine { get; set; }
    }
}
