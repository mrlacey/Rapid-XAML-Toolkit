// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class CalendarDatePickerHeaderAction : HardCodedStringAction
    {
        private CalendarDatePickerHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.CalendarDatePicker, Attributes.Header)
        {
        }

        public static CalendarDatePickerHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new CalendarDatePickerHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
