// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class ReplaceElementTests
    {
        [TestMethod]
        public void ReplaceSelfClosingElement()
        {
            var input = @"<Page>
    <WebView />
</Page>";

            var expected = @"<Page>
    <MyNewElement />
</Page>";

            this.TestWebViewReplaceElementConversions(input, expected);
        }

        [TestMethod]
        public void ReplaceElementWithChildAttributes()
        {
            var input = @"<Page>
    <WebView>
        <WebView.OnLoaded>OnLoadedEventHandler</WebView.OnLoaded>
    </WebView>
</Page>";

            var expected = @"<Page>
    <MyNewElement />
</Page>";

            this.TestWebViewReplaceElementConversions(input, expected);
        }

        private void TestWebViewReplaceElementConversions(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, _) = sut.ConvertFile("somefile.xaml", new[] { new WebViewReplaceElementAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class WebViewReplaceElementAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                return AutoFixAnalysisActions.ReplaceElement("<MyNewElement />");
            }
        }
    }
}
