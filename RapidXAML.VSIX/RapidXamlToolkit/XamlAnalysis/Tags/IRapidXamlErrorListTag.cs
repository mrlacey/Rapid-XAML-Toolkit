// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlErrorListTag : IRapidXamlAdornmentTag
    {
        string Message { get; set; }

        string ExtendedMessage { get; set; }

        int Line { get; set; }

        int Column { get; set; }

        string ErrorCode { get; set; }

        bool IsMessage { get; }

        bool IsError { get; }

        ErrorRow AsErrorRow();
    }
}
