// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class ColorHelperTests
    {
        [TestMethod]
        public void Invalid_HandledCorrectly()
        {
            var actual = ColorHelper.GetColor("invalid");

            Assert.IsNull(actual);
        }
    }
}
