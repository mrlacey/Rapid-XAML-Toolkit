// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class AddXmlnsTests
    {
        [TestMethod]
        public void AddWhenNoXmlns()
        {
            var input = @"<Page>
    <WebView />
</Page>";

            var expected = @"<Page xmlns:newcontrols=""https://somenewdomain/newcontrols"">
    <WebView />
</Page>";

            this.TestAddingXmlns(input, expected);
        }

        [TestMethod]
        public void AddWhenOtherXmlnsAlreadyExist()
        {
            var input = @"<Page xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
    <WebView />
</Page>";

            var expected = @"<Page xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:newcontrols=""https://somenewdomain/newcontrols"">
    <WebView />
</Page>";

            this.TestAddingXmlns(input, expected);
        }

        [TestMethod]
        public void DontAddWhenAlreadyThere()
        {
            var input = @"<Page xmlns:newcontrols=""https://somenewdomain/newcontrols"">
    <WebView />
</Page>";

            var expected = @"<Page xmlns:newcontrols=""https://somenewdomain/newcontrols"">
    <WebView />
</Page>";

            this.TestAddingXmlns(input, expected);
        }

        private void TestAddingXmlns(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, details) = sut.ConvertFile("somefile.xaml", new[] { new AddXmlnsAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.IsTrue(details.Count() > 3, $"Details count = {details.Count()}");
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class AddXmlnsAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                return AutoFixAnalysisActions.AddXmlns("newcontrols", "https://somenewdomain/newcontrols");
            }
        }
    }
}
