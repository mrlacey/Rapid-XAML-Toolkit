// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class HubHeaderAction : HardCodedStringAction
    {
        private HubHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.ComboBox, Attributes.Header)
        {
        }

        public static HubHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HubHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
