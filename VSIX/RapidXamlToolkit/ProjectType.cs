// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;

namespace RapidXamlToolkit
{
    [Flags]
    public enum ProjectType
    {
        Unknown = 1,

        [Description("UWP")]
        Uwp = 2,

        [Description("WPF")]
        Wpf = 4,

        [Description("Xamarin.Forms")]
        XamarinForms = 8,

        Any = Uwp | Wpf | XamarinForms,
    }
}
