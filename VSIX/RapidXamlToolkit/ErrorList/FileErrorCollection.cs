// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.ErrorList
{
    public class FileErrorCollection
    {
        public string FilePath { get; set; }

        public string Project { get; set; }

        public List<ErrorRow> Errors { get; set; } = new List<ErrorRow>();
    }
}
