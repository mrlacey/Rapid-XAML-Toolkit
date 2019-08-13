// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Options
{
    public class ProfileFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activeIndicator = StringRes.UI_ActiveProfileName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            var fallBackIndicator = StringRes.UI_FallBackProfileName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();

            if (value.ToString().EndsWith(activeIndicator)
             || value.ToString().EndsWith(fallBackIndicator))
            {
                return FontWeights.Bold;
            }
            else
            {
                return FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
