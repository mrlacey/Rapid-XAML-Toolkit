// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.AutoFix;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class XamlConverterTests
    {
        [TestMethod]
        public void ReplaceSelfClosingElementName()
        {
            var input = @"<Page>
<WebView />
</Page>";

            var expected = @"<Page>
<WebView2 />
</Page>";

            this.TestWebViewToWebView2BasicConversions(input, expected);
        }

        [TestMethod]
        public void ReplaceEmptyElementName()
        {
            var input = @"<Page>
<WebView></WebView>
</Page>";

            var expected = @"<Page>
<WebView2></WebView2>
</Page>";

            this.TestWebViewToWebView2BasicConversions(input, expected);
        }

        [TestMethod]
        public void ReplaceSelfClosingElementName_WithAttribute()
        {
            var input = @"<Page>
<WebView OnLoaded=""DoSomething"" />
</Page>";

            var expected = @"<Page>
<WebView2 OnLoaded=""DoSomething"" />
</Page>";

            this.TestWebViewToWebView2BasicConversions(input, expected);
        }

        [TestMethod]
        public void ReplaceEmptyElementName_WithAttribute()
        {
            var input = @"<Page>
<WebView OnLoaded=""DoSomething""></WebView>
</Page>";

            var expected = @"<Page>
<WebView2 OnLoaded=""DoSomething""></WebView2>
</Page>";

            this.TestWebViewToWebView2BasicConversions(input, expected);
        }

        [TestMethod]
        public void ReplaceSelfClosingElementName_WithXmlns()
        {
            var input = @"<Page>
<uwp:WebView />
</Page>";

            var expected = @"<Page>
<WebView2 />
</Page>";

            this.TestWebViewToWebView2BasicConversions(input, expected);
        }

        [TestMethod]
        public void ReplaceEmptyElementName_WithXmlns()
        {
            var input = @"<Page>
<uwp:WebView></uwp:WebView>
</Page>";

            var expected = @"<Page>
<WebView2></WebView2>
</Page>";

            this.TestWebViewToWebView2BasicConversions(input, expected);
        }

        [TestMethod]
        public void ReplaceElementName_WithChildren()
        {
            var input = @"<Page>
    <WebView>
        <SomethingElse />
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView2>
        <SomethingElse />
    </WebView2>
</Page>";

            this.TestWebViewToWebView2BasicConversions(input, expected);
        }

        private void TestWebViewToWebView2BasicConversions(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, _) = sut.ConvertFile("somefile.xaml", new[] { new WebViewToWebView2Basic() });

            Assert.AreEqual(true, success);
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class WebViewToWebView2Basic : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                return AutoFixAnalysisActions.RenameElement("WebView2");
            }
        }
    }
}
