// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlTag
    {
        (int Start, int Length) Span { get; set; }
    }
}
