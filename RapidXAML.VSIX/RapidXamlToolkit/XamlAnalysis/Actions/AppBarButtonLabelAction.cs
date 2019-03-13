// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AppBarButtonLabelAction : HardCodedStringAction
    {
        private AppBarButtonLabelAction(string file, ITextView textView)
            : base(file, textView, Elements.AppBarButton, Attributes.Label)
        {
        }

        public static AppBarButtonLabelAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new AppBarButtonLabelAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
