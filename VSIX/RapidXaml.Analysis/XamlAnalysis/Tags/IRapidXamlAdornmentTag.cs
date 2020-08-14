// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text.Tagging;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlAdornmentTag : IRapidXamlTag
    {
        string ToolTip { get; set; }

        IRapidXamlTextSnapshot Snapshot { get; set; }

        ITagSpan<IErrorTag> AsErrorTag();
    }
}
