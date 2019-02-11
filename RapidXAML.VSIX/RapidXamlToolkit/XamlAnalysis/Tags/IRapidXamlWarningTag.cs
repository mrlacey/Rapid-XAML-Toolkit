// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlWarningTag : IRapidXamlTag
    {
        string ToolTip { get; set; }

        string Message { get; set; }

        int Line { get; set; }

        int Column { get; set; }

        ITextSnapshot Snapshot { get; set; }

        ITagSpan<IErrorTag> AsErrorTag();

        ErrorRow AsErrorRow();
    }
}
