// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    public class AnalyzerTestsBase
    {
        internal AnalysisActions GetActions<T>(string xaml, ProjectType projectType = ProjectType.Any)
            where T : ICustomAnalyzer
        {
            var sut = (T)Activator.CreateInstance(typeof(T));

            var element = RapidXamlElementExtractor.GetElement(xaml);

            var details = new ExtraAnalysisDetails(
                "test.xaml",
                ProjectFrameworkHelper.FromType(projectType));

            var result = sut.Analyze(element, details);

            return result;
        }
    }
}
