// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class MediaElementProcessorTests : ProcessorTestsBase
    {
        [TestMethod]
        public void DeprecatedType_Detected()
        {
            var xaml = @"<MediaElement />";

            var outputTags = this.GetTags<MediaElementProcessor>(xaml);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<UseMediaPlayerElementTag>().Count());
        }
    }
}
