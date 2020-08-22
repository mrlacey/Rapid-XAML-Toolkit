// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidXaml.ApplyChanges
{
    public static class XamlConverter
    {
        public static (bool success, List<string> details) ConvertFile(string xamlFilePath, IEnumerable<ICustomAnalyzer> analyzers)
        {
            // TODO
        }

        public static (bool success, List<string> details) ConvertAllFilesInProject(string projectFilePath, IEnumerable<ICustomAnalyzer> analyzers)
        {
            throw new NotImplementedException("TODO");
        }
    }
}
