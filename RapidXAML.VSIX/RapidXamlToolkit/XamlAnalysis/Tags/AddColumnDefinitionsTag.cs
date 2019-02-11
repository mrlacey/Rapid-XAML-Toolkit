// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddColumnDefinitionsTag : LineInsertionTag
    {
        public AddColumnDefinitionsTag(Span span, ITextSnapshot snapshot)
            : base(span, snapshot)
        {
        }

        public new ActionTypes ActionType => ActionTypes.AddColumnDefinitions;
    }
}
