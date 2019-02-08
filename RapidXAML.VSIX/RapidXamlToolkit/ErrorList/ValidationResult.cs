// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.ErrorList
{
    class ValidationResult
    {
        public string Url { get; set; }

        public string Project { get; set; }

        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Extract { get; set; }

        public string Message { get; set; }
    }
}
