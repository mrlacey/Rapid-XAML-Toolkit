// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

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
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = original.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(original.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.SwapReplacements(
                original,
                InsertRowDefinitionAction.GetReplacements(1, 3),
                exclusions);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SwapReplacements_NothingToReplace()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = original.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(original.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.SwapReplacements(
                original,
                InsertRowDefinitionAction.GetReplacements(1, 3),
                exclusions);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SwapReplacements_NotAllIndexesDefined()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubFooter\" Grid.Row=\"3\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubSubFooter\" Grid.Row=\"4\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubSubSubFooter\" Grid.Row=\"5\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubSubSubSubFooter\" Grid.Row=\"6\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubFooter\" Grid.Row=\"3\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubSubFooter\" Grid.Row=\"4\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubSubSubFooter\" Grid.Row=\"5\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SubSubSubSubFooter\" Grid.Row=\"6\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = original.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(original.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.SwapReplacements(
                original,
                InsertRowDefinitionAction.GetReplacements(1, 3),
                exclusions);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SwapReplacements_MultipleIndexInstances()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body1\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body2\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer1\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer2\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body1\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body2\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer1\" Grid.Row=\"3\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer2\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = original.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(original.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.SwapReplacements(
                original,
                InsertRowDefinitionAction.GetReplacements(1, 3),
                exclusions);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SwapReplacements_MultipleIndexInstances_NotInOrder()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Body2\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer1\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body1\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer2\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Body2\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer1\" Grid.Row=\"3\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Body1\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <TextBlock Text=\"Footer2\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = original.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(original.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.SwapReplacements(
                original,
                InsertRowDefinitionAction.GetReplacements(1, 3),
                exclusions);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SwapReplacements_IgnoresSingleExclusion()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <Grid>"
   + Environment.NewLine + "            <Grid.RowDefinitions>"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            </Grid.RowDefinitions>"
   + Environment.NewLine + "            <TextBlock Text=\"nested-Body\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <Grid>"
   + Environment.NewLine + "            <Grid.RowDefinitions>"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            </Grid.RowDefinitions>"
   + Environment.NewLine + "            <TextBlock Text=\"nested-Body\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = original.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(original.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.SwapReplacements(
                original,
                InsertRowDefinitionAction.GetReplacements(1, 3),
                exclusions);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SwapReplacements_IgnoresMultipleExclusions()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <Grid>"
   + Environment.NewLine + "            <Grid.RowDefinitions>"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            </Grid.RowDefinitions>"
   + Environment.NewLine + "            <TextBlock Text=\"nested-Body1\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + "        <Grid>"
   + Environment.NewLine + "            <Grid.RowDefinitions>"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            </Grid.RowDefinitions>"
   + Environment.NewLine + "            <TextBlock Text=\"nested-Body2\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Header\" Grid.Row=\"0\" />"
   + Environment.NewLine + "        <Grid>"
   + Environment.NewLine + "            <Grid.RowDefinitions>"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            </Grid.RowDefinitions>"
   + Environment.NewLine + "            <TextBlock Text=\"nested-Body1\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + "        <Grid>"
   + Environment.NewLine + "            <Grid.RowDefinitions>"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "                <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            </Grid.RowDefinitions>"
   + Environment.NewLine + "            <TextBlock Text=\"nested-Body2\" Grid.Row=\"1\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = original.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(original.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.SwapReplacements(
                original,
                InsertRowDefinitionAction.GetReplacements(1, 3),
                exclusions);

            StringAssert.AreEqual(expected, actual);
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
