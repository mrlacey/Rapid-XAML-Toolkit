// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows.Data;

namespace RapidXamlToolkit.Options
{
    public class ProjectTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.ToString().Equals(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value?.Equals(true) == true)
            {
                switch (parameter)
                {
                    case "Uwp": return ProjectType.Uwp;
                    case "Wpf": return ProjectType.Wpf;
                    case "XamarinForms": return ProjectType.XamarinForms;
                    case "WinUI": return ProjectType.WinUI;
                    case "MAUI": return ProjectType.MAUI;
                    default: return Binding.DoNothing;
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}
