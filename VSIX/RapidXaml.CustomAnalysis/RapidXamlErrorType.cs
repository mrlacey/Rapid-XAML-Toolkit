// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public enum RapidXamlErrorType
    {
        /// <summary>
        /// Report as a 'Message' on the ErrorList and be underlined with only a hint.
        /// </summary>
        Suggestion,

        /// <summary>
        /// Report as a 'Warning' on the ErrorList and be underlined in green.
        /// </summary>
        Warning,

        /// <summary>
        /// Report as an 'Error' on the ErrorList and be underlined in red.
        /// </summary>
        Error,
    }
}
