// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlErrorListTag : IRapidXamlAdornmentTag
    {
        string Description { get; set; }

        string ExtendedMessage { get; set; }

        int Line { get; }

        int Column { get; }

        string ErrorCode { get; }

        bool IsMessage { get; }

        bool IsError { get; }

        ErrorRow AsErrorRow();
    }
}
