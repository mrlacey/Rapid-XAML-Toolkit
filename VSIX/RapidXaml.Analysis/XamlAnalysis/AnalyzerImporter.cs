// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class AnalyzerImporter
    {
        [ImportMany(typeof(ICustomAnalyzer), AllowRecomposition =true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        public IEnumerable<ICustomAnalyzer> CustomAnalyzers { get; set; }
    }
}
