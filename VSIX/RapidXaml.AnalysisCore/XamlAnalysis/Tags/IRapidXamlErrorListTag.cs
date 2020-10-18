// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public interface IRapidXamlErrorListTag : IRapidXamlAdornmentTag
    {
        string Description { get; set; }

        string ExtendedMessage { get; set; }

        int Line { get; }

        int Column { get; }

        bool IsInternalError { get; }
    }
}
