// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ColumnSpanOverflowAction : AddMissingColumnDefinitionsAction
    {
        private ColumnSpanOverflowAction(string file)
            : base(file)
        {
        }

        public static ColumnSpanOverflowAction Create(ColumnSpanOverflowTag tag, string file)
        {
            var result = new ColumnSpanOverflowAction(file)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
