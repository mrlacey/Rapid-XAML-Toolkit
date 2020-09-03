// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class CombinationTests
    {
        [TestMethod]
        public void MultipleActions_SelfClosing()
        {
            var input = @"<Page>
    <WebView />
</Page>";

            var expected = @"<Page>
    <WebView2 Source=""https://rapidxaml.dev/"" />
</Page>";

            this.TestWebViewMultipleActionConversions(input, expected);
        }

        [TestMethod]
        public void MultipleActions()
        {
            var input = @"<Page>
    <WebView></WebView>
</Page>";

            var expected = @"<Page>
    <WebView2 Source=""https://rapidxaml.dev/""></WebView2>
</Page>";

            this.TestWebViewMultipleActionConversions(input, expected);
        }

        private void TestWebViewMultipleActionConversions(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, details) = sut.ConvertFile("somefile.xaml", new[] { new WebViewMultipleActionsAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.IsTrue(details.Count() > 3);
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class WebViewMultipleActionsAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                var result = AutoFixAnalysisActions.RenameElement("WebView2");

                result.AndAddAttribute("Source", "https://rapidxaml.dev/");

                return result;
            }
        }
    }
}
