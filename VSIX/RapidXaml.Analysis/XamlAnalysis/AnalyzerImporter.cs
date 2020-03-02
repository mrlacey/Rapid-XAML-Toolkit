// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class AnalyzerImporter
    {
        [ImportMany]
        public IEnumerable<Lazy<RapidXaml.ICustomAnalyzer>> CustomAnalyzers { get; set; }
    }
}
