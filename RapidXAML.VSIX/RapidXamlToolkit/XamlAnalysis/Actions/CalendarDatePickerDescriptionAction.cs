// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class CalendarDatePickerDescriptionAction : HardCodedStringAction
    {
        private CalendarDatePickerDescriptionAction(string file, ITextView textView)
            : base(file, textView, Elements.CalendarDatePicker, Attributes.Description)
        {
        }

        public static CalendarDatePickerDescriptionAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new CalendarDatePickerDescriptionAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
