// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXaml;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    public class AnalyzerTestsBase
    {
        internal AnalysisActions GetActions<T>(string xaml, ProjectType projectType = ProjectType.Any)
            where T : BuiltInXamlAnalyzer
        {
            var sut = this.CreateAnalyzer<T>();

            var element = RapidXamlElementExtractor.GetElement(xaml);

            var details = new ExtraAnalysisDetails(
                "test.xaml",
                ProjectFrameworkHelper.FromType(projectType));

            var result = sut.Analyze(element, details);

            return result;
        }

        internal BuiltInXamlAnalyzer CreateAnalyzer<T>()
            where T : BuiltInXamlAnalyzer
        {
            return (T)Activator.CreateInstance(typeof(T), new TestVisualStudioAbstraction(), DefaultTestLogger.Create());
        }

        internal List<AnalysisAction> Act<T>(string xaml, ProjectFramework framework = ProjectFramework.Unknown)
            where T : BuiltInXamlAnalyzer
        {
            var sut = this.CreateAnalyzer<T>();
            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(framework));

            return actual.Actions;
        }
    }
}
