// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class AddChildStringTests
    {
        [TestMethod]
        public void AddToSelfClosingElement()
        {
            var input = @"<Page>
    <WebView />
</Page>";

            var expected = @"<Page>
    <WebView >
<WebView.SomeProperty>SomeValue</WebView.SomeProperty>
</WebView>
</Page>";

            this.TestWebViewAddChildStringConversions(input, expected);
        }

        [TestMethod]
        public void AddToSelfClosingElementWithAttributes()
        {
            var input = @"<Page>
    <WebView Name=""TheWebView"" />
</Page>";

            var expected = @"<Page>
    <WebView Name=""TheWebView"" >
<WebView.SomeProperty>SomeValue</WebView.SomeProperty>
</WebView>
</Page>";

            this.TestWebViewAddChildStringConversions(input, expected);
        }

        [TestMethod]
        public void AddToElement()
        {
            var input = @"<Page>
    <WebView>
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView>
<WebView.SomeProperty>SomeValue</WebView.SomeProperty>
    </WebView>
</Page>";

            this.TestWebViewAddChildStringConversions(input, expected);
        }

        [TestMethod]
        public void AddToElementWithAttribute()
        {
            var input = @"<Page>
    <WebView Name=""TheWebView"">
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView Name=""TheWebView"">
<WebView.SomeProperty>SomeValue</WebView.SomeProperty>
    </WebView>
</Page>";

            this.TestWebViewAddChildStringConversions(input, expected);
        }

        [TestMethod]
        public void AddToElementWithAttributeAndChild()
        {
            var input = @"<Page>
    <WebView Name=""TheWebView"">
        <WebView.AProperty>AValue</WebView.AProperty>
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView Name=""TheWebView"">
<WebView.SomeProperty>SomeValue</WebView.SomeProperty>
        <WebView.AProperty>AValue</WebView.AProperty>
    </WebView>
</Page>";

            this.TestWebViewAddChildStringConversions(input, expected);
        }

        private void TestWebViewAddChildStringConversions(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, _) = sut.ConvertFile("somefile.xaml", new[] { new WebViewAddChildStringAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class WebViewAddChildStringAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                return AutoFixAnalysisActions.AddChildString("<WebView.SomeProperty>SomeValue</WebView.SomeProperty>");
            }
        }
    }
}
