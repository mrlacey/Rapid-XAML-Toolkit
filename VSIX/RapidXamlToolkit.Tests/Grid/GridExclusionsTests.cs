// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.Tests.Grid
{
    [TestClass]
    public class GridExclusionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GetGridBoundary_NoOtherGrids()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_SingleNestedGrid()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">☆"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>☆"
+ Environment.NewLine + "    </Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_SingleNestedEmptyGrid_NotTreatedAsExclusion()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"1\" />"
+ Environment.NewLine + "    </Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_MultipleNestedGrids()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">☆"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>☆"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"2\">☆"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>☆"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_MultipleLevelNestedGrids()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"0\">☆"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + "            <Grid Grid.Row=\"0\">"
+ Environment.NewLine + "                <Grid.RowDefinitions>"
+ Environment.NewLine + "                    <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                    <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "                </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "                <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "            </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>☆"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">☆"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>☆"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_MultipleLevelNestedGrids_NestingNotFirst()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "</Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_MultipleMultipleLevelNestedGrids()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "</Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_MultipleLevelNestedGrids_VeryDeep()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <Grid>"
+ Environment.NewLine + "                <Grid>"
+ Environment.NewLine + "                </Grid>"
+ Environment.NewLine + "            </Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "</Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_SelfClosingThen_MultipleLevelNestedGrids_VeryDeep()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Grid />"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <Grid>"
+ Environment.NewLine + "                <Grid>"
+ Environment.NewLine + "                </Grid>"
+ Environment.NewLine + "            </Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "</Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_MultipleDeepNestedChildGrids_VaryingDepths()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <Grid />"
+ Environment.NewLine + "            <Grid />"
+ Environment.NewLine + "            <Grid>"
+ Environment.NewLine + "                <Grid>"
+ Environment.NewLine + "                    <Grid />"
+ Environment.NewLine + "                </Grid>"
+ Environment.NewLine + "                <Grid>"
+ Environment.NewLine + "                    <Grid />"
+ Environment.NewLine + "                </Grid>"
+ Environment.NewLine + "            </Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "</Grid>";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        [TestMethod]
        public void GetGridBoundary_MultipleDeepNestedChildGrids_VaryingDepths_Plus_TrailingWhitespace()
        {
            var xaml = "<Grid>"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <Grid />"
+ Environment.NewLine + "            <Grid />"
+ Environment.NewLine + "            <Grid>"
+ Environment.NewLine + "                <Grid>"
+ Environment.NewLine + "                    <Grid />"
+ Environment.NewLine + "                </Grid>"
+ Environment.NewLine + "                <Grid>"
+ Environment.NewLine + "                    <Grid />"
+ Environment.NewLine + "                </Grid>"
+ Environment.NewLine + "            </Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "    <Grid>☆"
+ Environment.NewLine + "    </Grid>☆"
+ Environment.NewLine + "</Grid>    "
+ Environment.NewLine + "       "
+ Environment.NewLine + "      "
+ Environment.NewLine + "          "
+ Environment.NewLine + "     "
+ Environment.NewLine + ""
+ Environment.NewLine + "";

            this.ShouldReturnExclusionsMarkedByStars(xaml);
        }

        private void ShouldReturnExclusionsMarkedByStars(string xamlWithStars)
        {
            // Stars mark the boundaries of the excluded areas - there should always be an even number
            Assert.AreEqual(0, xamlWithStars.Count(c => c.ToString() == "☆") % 2, "Missing '☆' in xaml.");

            var expected = new Dictionary<int, int>() { };

            var actualXaml = xamlWithStars;

            while (actualXaml.Contains("☆"))
            {
                var areaStart = actualXaml.IndexOf("☆", StringComparison.Ordinal);
                var areaEnd = actualXaml.IndexOf("☆", areaStart + 1, StringComparison.Ordinal) - 1; // remove 1 to account for the opening placeholder

                expected.Add(areaStart, areaEnd);

                actualXaml = actualXaml.Remove(areaStart, 1);
                actualXaml = actualXaml.Remove(areaEnd, 1);
            }

            var exclusions = InsertRowDefinitionAction.GetExclusions(actualXaml);

            Assert.IsNotNull(exclusions);
            Assert.AreEqual(expected.Count, exclusions.Count);

            foreach (var expectedKey in expected.Keys)
            {
                Assert.IsTrue(exclusions.ContainsKey(expectedKey), $"Missing expected exclusion starting {expectedKey}");
                Assert.AreEqual(exclusions[expectedKey], expected[expectedKey], $"Missing expected exclusion ending {expected[expectedKey]}");
            }
        }
    }
}
