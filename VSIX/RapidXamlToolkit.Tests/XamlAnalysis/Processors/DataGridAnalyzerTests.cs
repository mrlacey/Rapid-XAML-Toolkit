// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class DataGridAnalyzerTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void NoBinding_NothingDetected()
        {
            var xaml = @"<DataGrid SelectedItem=""NoBinding"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void TwoWayBinding_NothingDetected()
        {
            var xaml = @"<DataGrid SelectedItem=""{Binding SelItem, Mode=TwoWay}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void TwoWayXbind_NothingDetected()
        {
            var xaml = @"<DataGrid SelectedItem=""{x:Bind SelItem, Mode=TwoWay}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void OneWayBinding_Detected()
        {
            var xaml = @"<DataGrid SelectedItem=""{Binding SelItem, Mode=OneWay}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void OneWayXbind_Detected()
        {
            var xaml = @"<DataGrid SelectedItem=""{x:Bind SelItem, Mode=OneWay}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void OneTimeBinding_Detected()
        {
            var xaml = @"<DataGrid SelectedItem=""{Binding SelItem, Mode=OneTime}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void OneTimeXbind_Detected()
        {
            var xaml = @"<DataGrid SelectedItem=""{x:Bind SelItem, Mode=OneTime}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void DefaultBindingMode_Detected()
        {
            var xaml = @"<DataGrid SelectedItem=""{Binding SelItem}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(0, outputTags.Count);
            Assert.AreEqual(0, outputTags.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        [TestMethod]
        public void DefaultXbindMode_Detected()
        {
            var xaml = @"<DataGrid SelectedItem=""{x:Bind SelItem}"" />";

            var outputTags = this.GetTags(xaml);

            Assert.AreEqual(0, outputTags.Count);
            Assert.AreEqual(0, outputTags.Count(a => a.Action == ActionType.ReplaceAttributeValue));
        }

        private List<AnalysisAction> GetTags(string xaml)
        {
            return this.Act<DataGridAnalyzer>(xaml, ProjectFramework.Uwp);
        }
    }
}
