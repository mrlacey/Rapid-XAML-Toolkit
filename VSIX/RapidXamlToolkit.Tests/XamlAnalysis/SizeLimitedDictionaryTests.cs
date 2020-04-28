// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class SizeLimitedDictionaryTests
    {
        [TestMethod]
        public void CachedItems_Found()
        {
            var sut = new SizeLimitedDictionary<string, int>(2);

            sut.Add("one", 1);

            Assert.IsTrue(sut.ContainsKey("one"));
        }

        [TestMethod]
        public void UncachedItems_NotFound()
        {
            var sut = new SizeLimitedDictionary<string, int>(2);

            Assert.IsFalse(sut.ContainsKey("one"));
        }

        [TestMethod]
        public void CanAddMoreItemsThanLimit()
        {
            var sut = new SizeLimitedDictionary<string, int>(2);

            sut.Add("one", 1);
            sut.Add("two", 2);
            sut.Add("three", 3);
        }

        [TestMethod]
        public void OldestCacheItemsRemovedFirst()
        {
            var sut = new SizeLimitedDictionary<string, int>(2);

            sut.Add("one", 1);
            sut.Add("two", 2);
            sut.Add("three", 3);

            Assert.IsFalse(sut.ContainsKey("one"));
        }
    }
}
