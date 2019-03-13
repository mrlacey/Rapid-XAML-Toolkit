// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class DatePickerHeaderAction : HardCodedStringAction
    {
        private DatePickerHeaderAction(string file, ITextView textView)
            : base(file, textView, Elements.DatePicker, Attributes.Header)
        {
        }

        public static DatePickerHeaderAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new DatePickerHeaderAction(file, view)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
