// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit.XamlAnalysis
{
    [Flags]
    public enum AttributeType
    {
        None = 0,
        Inline = 1,
        Element = 2,
        DefaultValue = 4,
        Any = 7,
    }
}
