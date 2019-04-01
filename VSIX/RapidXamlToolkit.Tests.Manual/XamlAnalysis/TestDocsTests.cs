// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.Manual.XamlAnalysis
{
    [TestClass]
    public class TestDocsTests
    {
        [TestMethod]
        public void Generic()
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(".\\XamlAnalysis\\TestDocs\\Generic.xaml");

            var snapshot = new RapidXamlToolkit.Tests.FakeTextSnapshot();

            XamlElementExtractor.Parse(snapshot, text, RapidXamlDocument.GetAllProcessors(), result.Tags);

            Assert.AreEqual(0, result.Tags.OfType<MissingRowDefinitionTag>().Count());
            Assert.AreEqual(0, result.Tags.OfType<MissingColumnDefinitionTag>().Count());
        }
    }
}
