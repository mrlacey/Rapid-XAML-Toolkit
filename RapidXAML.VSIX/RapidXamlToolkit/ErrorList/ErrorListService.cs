// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.ErrorList
{
    public class ErrorListService
    {
        public static void Process(FileErrorCollection result)
        {
            // TODO: need to update this for Rapid XAML warnings
            TableDataSource.Instance.CleanErrors(result.FilePath);
            TableDataSource.Instance.AddErrors(result);
        }
    }
}
