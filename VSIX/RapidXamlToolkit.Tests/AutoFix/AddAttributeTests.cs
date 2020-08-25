// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class AddAttributeTests
    {
        [TestMethod]
        public void AddToEmptySelfClosingElement()
        {
            var input = @"<Page>
    <WebView />
</Page>";

            var expected = @"<Page>
    <WebView OnLoaded=""OnLoadedEventHandler"" />
</Page>";

            this.TestWebViewAddAttributeConversions(input, expected);
        }

        [TestMethod]
        public void NoChangeIfAlreadyInSelfClosingElement()
        {
            var input = @"<Page>
    <WebView OnLoaded=""OnLoadedEventHandler"" />
</Page>";

            var expected = @"<Page>
    <WebView OnLoaded=""OnLoadedEventHandler"" />
</Page>";

            this.TestWebViewAddAttributeConversions(input, expected);
        }

        [TestMethod]
        public void AddIfOtherAttributesInSelfClosingElement()
        {
            var input = @"<Page>
    <WebView x:Name=""WebViewName"" />
</Page>";

            var expected = @"<Page>
    <WebView x:Name=""WebViewName"" OnLoaded=""OnLoadedEventHandler"" />
</Page>";

            this.TestWebViewAddAttributeConversions(input, expected);
        }

        [TestMethod]
        public void AddToEmptyElement()
        {
            var input = @"<Page>
    <WebView></WebView>
</Page>";

            var expected = @"<Page>
    <WebView OnLoaded=""OnLoadedEventHandler""></WebView>
</Page>";

            this.TestWebViewAddAttributeConversions(input, expected);
        }

        [TestMethod]
        public void NoChangeIfAlreadyInEmptyElement()
        {
            var input = @"<Page>
    <WebView OnLoaded=""OnLoadedEventHandler""></WebView>
</Page>";

            var expected = @"<Page>
    <WebView OnLoaded=""OnLoadedEventHandler""></WebView>
</Page>";

            this.TestWebViewAddAttributeConversions(input, expected);
        }

        [TestMethod]
        public void AddIfOtherAttributesInEmptyElement()
        {
            var input = @"<Page>
    <WebView x:Name=""WebViewName""></WebView>
</Page>";

            var expected = @"<Page>
    <WebView OnLoaded=""OnLoadedEventHandler"" x:Name=""WebViewName""></WebView>
</Page>";

            this.TestWebViewAddAttributeConversions(input, expected);
        }

        private void TestWebViewAddAttributeConversions(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, _) = sut.ConvertFile("somefile.xaml", new[] { new WebViewAddAttributeAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class WebViewAddAttributeAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                if (!element.ContainsAttribute("OnLoaded"))
                {
                    return AutoFixAnalysisActions.AddAttribute("OnLoaded", "OnLoadedEventHandler");
                }
                else
                {
                    return AutoFixAnalysisActions.None;
                }
            }
        }
    }
}
