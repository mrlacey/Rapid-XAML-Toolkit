// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.Tests.Grid
{
    [TestClass]
    public class GridReplacementTests
    {
        [TestMethod]
        public void GetExpectedReplacementsFromFirstDefinition()
        {
            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"6\"", " Grid.Row=\"7\""),
                (" Grid.Row=\"5\"", " Grid.Row=\"6\""),
                (" Grid.Row=\"4\"", " Grid.Row=\"5\""),
                (" Grid.Row=\"3\"", " Grid.Row=\"4\""),
                (" Grid.Row=\"2\"", " Grid.Row=\"3\""),
                (" Grid.Row=\"1\"", " Grid.Row=\"2\""),
                (" Grid.Row=\"0\"", " Grid.Row=\"1\""),
            };

            this.GeneratesExpectedReplacements(0, 7, expected);
        }

        [TestMethod]
        public void GetExpectedReplacementsFromMiddleDefinition()
        {
            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"6\"", " Grid.Row=\"7\""),
                (" Grid.Row=\"5\"", " Grid.Row=\"6\""),
                (" Grid.Row=\"4\"", " Grid.Row=\"5\""),
                (" Grid.Row=\"3\"", " Grid.Row=\"4\""),
                (" Grid.Row=\"2\"", " Grid.Row=\"3\""),
                (" Grid.Row=\"1\"", " Grid.Row=\"2\""),
            };

            this.GeneratesExpectedReplacements(1, 7, expected);
        }

        [TestMethod]
        public void GetExpectedReplacementsFromLastDefinition()
        {
            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"6\"", " Grid.Row=\"7\""),
            };

            this.GeneratesExpectedReplacements(6, 7, expected);
        }

        [TestMethod]
        public void GetExpectedReplacementsIgnoresSubsequentNestedGrids()
        {
            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"2\"", " Grid.Row=\"3\""),
                (" Grid.Row=\"1\"", " Grid.Row=\"2\""),
            };

            this.GeneratesExpectedReplacements(1, 3, expected);
        }

        [TestMethod]
        public void SwapReplacements_Simple()
        {
            Assert.Inconclusive("Implement this test");
        }

        [TestMethod]
        public void SwapReplacements_NothingToReplace()
        {
            Assert.Inconclusive("Implement this test");
        }

        [TestMethod]
        public void SwapReplacements_NotAllIndexesDefined()
        {
            Assert.Inconclusive("Implement this test");
        }

        [TestMethod]
        public void SwapReplacements_MultipleIndexInstances()
        {
            Assert.Inconclusive("Implement this test");
        }

        [TestMethod]
        public void SwapReplacements_MultipleIndexInstances_NotInOrder()
        {
            Assert.Inconclusive("Implement this test");
        }

        [TestMethod]
        public void SwapReplacements_IgnoresSingleExclusion()
        {
            Assert.Inconclusive("Implement this test");
        }

        [TestMethod]
        public void SwapReplacements_IgnoresMultipleExclusions()
        {
            Assert.Inconclusive("Implement this test");
        }

        private void GeneratesExpectedReplacements(int row, int total, List<(string, string)> expected)
        {
            var actual = InsertRowDefinitionAction.GetReplacements(row, total);

            if (expected != null && actual != null)
            {
                // Assert isn't able to compare lists of tuples automatically so doing the heavy lifting directly
                Assert.AreEqual(expected.Count, actual.Count);

                for (int i = 0; i < expected.Count; i++)
                {
                    Assert.AreEqual(expected[i].Item1, actual[i].find);
                    Assert.AreEqual(expected[i].Item2, actual[i].replace);
                }
            }
        }
    }
}
