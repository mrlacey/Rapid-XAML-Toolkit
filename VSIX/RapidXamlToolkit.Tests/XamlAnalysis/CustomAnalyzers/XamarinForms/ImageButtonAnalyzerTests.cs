// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class ImageButtonAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void TestSomething()
        {
            var xaml = @"<ImageButton />";

            var actions = this.GetActions<ImageButtonAnalyzer>(xaml, ProjectType.XamarinForms);

            // TODO: Add a test once the analyzer does something
        }
    }
}
