// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class RemoveChildTests
    {
        [TestMethod]
        public void RemoveOnlyChild()
        {
            var input = @"<Page>
    <WebView>
        <TextBlock>SomeText</TextBlock>
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView>
        
    </WebView>
</Page>";

            this.TestWebViewRemoveChildConversions(input, expected);
        }

        [TestMethod]
        public void RemoveOnlyChild_SelfClosing()
        {
            var input = @"<Page>
    <WebView>
        <TextBlock />
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView>
        
    </WebView>
</Page>";

            this.TestWebViewRemoveChildConversions(input, expected);
        }

        [TestMethod]
        public void RemoveOneOfManyChildAttributes()
        {
            var input = @"<Page>
    <WebView>
        <TextBlock>SomeText</TextBlock>
        <Button />
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView>
        
        <Button />
    </WebView>
</Page>";

            this.TestWebViewRemoveChildConversions(input, expected);
        }

        [TestMethod]
        public void RemoveMultipleChildAttributes()
        {
            var input = @"<Page>
    <WebView>
        <TextBlock>SomeText</TextBlock>
        <Button />
        <TextBlock>SomeText</TextBlock>
    </WebView>
</Page>";

            var expected = @"<Page>
    <WebView>
        
        <Button />
        
    </WebView>
</Page>";

            this.TestWebViewRemoveChildConversions(input, expected);
        }

        private void TestWebViewRemoveChildConversions(string original, string expected)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = original,
            };

#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, _) = sut.ConvertFile("somefile.xaml", new[] { new WebViewRemoveChildAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.AreEqual(expected, fs.WrittenFileText);
#endif
        }

        public class WebViewRemoveChildAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                if (element.ContainsChild("TextBlock"))
                {
                    return AutoFixAnalysisActions.RemoveChild(element.Children.First(c => c.Name == "TextBlock"));
                }
                else
                {
                    return AutoFixAnalysisActions.None;
                }
            }
        }
    }
}
