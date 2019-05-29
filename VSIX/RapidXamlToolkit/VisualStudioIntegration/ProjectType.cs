// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    [Flags]
    public enum ProjectType
    {
        Unknown = 1,
        Uwp = 2,
        Wpf = 4,
        XamarinForms = 8,
        Any = Uwp | Wpf | XamarinForms,
    }
}
