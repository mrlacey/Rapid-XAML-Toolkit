// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.ErrorList
{
    public class ErrorListService
    {
        public static void Process(FileErrorCollection result)
        {
            TableDataSource.Instance.CleanErrors(result.FilePath);
            TableDataSource.Instance.AddErrors(result);
        }
    }
}
