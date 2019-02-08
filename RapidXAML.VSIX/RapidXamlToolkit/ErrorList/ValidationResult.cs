// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.ErrorList
{
    public class ValidationResult
    {
        public string FilePath { get; set; }

        public string Project { get; set; }

        public List<Error> Errors { get; set; } = new List<Error>();
    }
}
