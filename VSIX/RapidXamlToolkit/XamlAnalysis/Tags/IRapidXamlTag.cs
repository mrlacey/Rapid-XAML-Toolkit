// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlTag : ITag
    {
        Type SuggestedAction { get; }

        Span Span { get; set; }
    }
}
