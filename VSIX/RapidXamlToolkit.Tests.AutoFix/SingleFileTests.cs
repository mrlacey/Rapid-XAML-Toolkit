// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class SingleFileTests
    {
        [TestMethod]
        public void CanModifySpecifiedFile()
        {
            var xamlFile1 = @"<Page>
    <WebView />
</Page>";

            var expectedContent = @"<Page>
    <WebView2 Source=""https://rapidxaml.dev/"" />
</Page>";

            var fs = new BespokeTestFileSystem
            {
                FileExistsResponse = true,
                FileLines = xamlFile1.Split(new[] { Environment.NewLine }, StringSplitOptions.None),
            };

            var fileName = "Page1.xaml";

            fs.FilesAndContents.Add(fileName, xamlFile1);

            var sut = new XamlConverter(fs);

            var (success, details) = sut.ConvertFile(fileName, new[] { new WebViewMultipleActionsAnalyzer() });

            var newFileContents = fs.WrittenFiles[fileName];

            Assert.AreEqual(true, success);
            Assert.AreEqual(5, details.Count());
            Assert.AreEqual(expectedContent, newFileContents);
        }
    }
}
