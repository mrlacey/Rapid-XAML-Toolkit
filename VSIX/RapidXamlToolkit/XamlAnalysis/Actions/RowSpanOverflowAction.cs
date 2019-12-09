// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class RowSpanOverflowAction : AddMissingRowDefinitionsAction
    {
        public RowSpanOverflowAction(string file)
            : base(file)
        {
        }

        public static RowSpanOverflowAction Create(RowSpanOverflowTag tag, string file)
        {
            var result = new RowSpanOverflowAction(file)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
