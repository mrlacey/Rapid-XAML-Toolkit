﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class MenuFlyoutSubItemProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void HardCoded_Text_Detected()
        {
            var xaml = @"<MenuFlyoutSubItem Text=""HCValue"" />";

            var actual = this.Act<MenuFlyoutSubItemAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.CreateResource));
        }
    }
}
