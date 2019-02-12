// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlErrorListTag : IRapidXamlAdornmentTag
    {
        ErrorRow AsErrorRow();
    }

    public interface IRapidXamlAdornmentTag : IRapidXamlTag
    {
        string ToolTip { get; set; }

        string Message { get; set; }

        string ExtendedMessage { get; set; }

        int Line { get; set; }

        int Column { get; set; }

        ITextSnapshot Snapshot { get; set; }

        string ErrorCode { get; set; }

        bool IsMessage { get; }

        bool IsError { get; }

        ITagSpan<IErrorTag> AsErrorTag();
    }
}
