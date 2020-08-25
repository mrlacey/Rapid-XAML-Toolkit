// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class RemoveAttributeTests
    {
        [TestMethod]
        public void RemoveOnlyAttributeOfSelfClosingElement()
        {
            var input = @"<Page>
    <WebView OnLoaded=""LoadedHandler"" />
</Page>";

            var expected = @"<Page>
    <WebView  />
</Page>";

            this.TestWebViewRemoveAttributeConversions(input, expected);
        }

        [TestMethod]
        public void DoesNothingIfNotInSelfClosingElement()
        {
            var input = @"<Page>
    <WebView Name=""TheWebView"" />
</Page>";

            var expected = @"<Page>
    <WebView Name=""TheWebView"" />
</Page>";

            this.TestWebViewRemoveAttributeConversions(input, expected);
        }

        [TestMethod]
        public void RemovedIfOneOfManyInSelfClosingElement()
        {
            var input = @"<Page>
    <WebView Name=""TheWebView"" OnLoaded=""LoadedHandler"" />
</Page>";

            var expected = @"<Page>
    <WebView Name=""TheWebView""  />
</Page>";

            this.TestWebViewRemoveAttributeConversions(input, expected);
        }

        [TestMethod]
        public void RemoveOnlyChildAttribute()
        {
            var input = @"<Page>
    <WebView>
        <WebView.OnLoaded>OnLoadedEventHandler</WebView.OnLoaded>
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView>
        
    </WebView>
</Page>";

            this.TestWebViewRemoveAttributeConversions(input, expected);
        }

        [TestMethod]
        public void RemoveOneOfManyChildAttributes()
        {
            var input = @"<Page>
    <WebView>
        <WebView.SomeAttribute>SomeValue</WebView.SomeAttribute>
        <WebView.OnLoaded>OnLoadedEventHandler</WebView.OnLoaded>
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView>
        <WebView.SomeAttribute>SomeValue</WebView.SomeAttribute>
        
    </WebView>
</Page>";

            this.TestWebViewRemoveAttributeConversions(input, expected);
        }

        [TestMethod]
        public void RemoveOnlyAttribute()
        {
            var input = @"<Page>
    <WebView OnLoaded=""LoadedHandler"">
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView >
    </WebView>
</Page>";

            this.TestWebViewRemoveAttributeConversions(input, expected);
        }

        private void TestWebViewRemoveAttributeConversions(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, _) = sut.ConvertFile("somefile.xaml", new[] { new WebViewRemoveAttributeAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class WebViewRemoveAttributeAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                return AutoFixAnalysisActions.RemoveAttribute("OnLoaded");
            }
        }
    }
}
