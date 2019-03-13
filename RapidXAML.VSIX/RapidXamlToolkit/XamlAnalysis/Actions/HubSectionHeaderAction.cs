// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class HubSectionHeaderAction : HardCodedStringAction
    {
        private HubSectionHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.ComboBox, Attributes.Header)
        {
        }

        public static HubSectionHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HubSectionHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
