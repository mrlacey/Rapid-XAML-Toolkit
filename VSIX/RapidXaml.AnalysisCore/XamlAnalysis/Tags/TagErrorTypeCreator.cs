// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public static class TagErrorTypeCreator
    {
        public static TagErrorType FromCustomAnalysisErrorType(RapidXamlErrorType customType)
        {
            switch (customType)
            {
                case RapidXamlErrorType.Suggestion:
                    return TagErrorType.Suggestion;

                case RapidXamlErrorType.Warning:
                    return TagErrorType.Warning;

                case RapidXamlErrorType.Error:
                    return TagErrorType.Error;

                default:
                    return TagErrorType.Hidden;
            }
        }
    }
}
