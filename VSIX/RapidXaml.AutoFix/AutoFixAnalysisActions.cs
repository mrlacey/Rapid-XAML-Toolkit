// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public class AutoFixAnalysisActions : AnalysisActions
    {
        public static AnalysisActions RenameElement(string newName)
        {
            return RenameElement(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                newName);
        }
    }
}
