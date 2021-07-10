// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;

namespace RapidXaml
{
    public enum ProjectFramework
    {
        Unknown = 1,

        [Description("UWP")]
        Uwp = 2,

        [Description("WPF")]
        Wpf = 4,

        [Description("Xamarin.Forms")]
        XamarinForms = 8,

        [Description("WinUI3")]
        WinUI = 16,

        [Description(".NET MAUI")]
        Maui = 32,
    }
}
