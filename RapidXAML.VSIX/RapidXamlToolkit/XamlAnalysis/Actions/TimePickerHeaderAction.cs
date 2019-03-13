// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class TimePickerHeaderAction : HardCodedStringAction
    {
        private TimePickerHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.TimePicker, Attributes.Header)
        {
        }

        public static TimePickerHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new TimePickerHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
