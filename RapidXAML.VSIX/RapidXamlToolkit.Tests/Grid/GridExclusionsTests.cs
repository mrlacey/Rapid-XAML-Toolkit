// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
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

            var expected = new Dictionary<int, int>();

            this.ShouldReturnExpectedBoundary(xaml, expected);
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
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>";

            var expected = new Dictionary<int, int> { { 244, 542 } };

            this.ShouldReturnExpectedBoundary(xaml, expected);
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
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"2\">"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>";

            var expected = new Dictionary<int, int> { { 244, 542 }, { 554, 798 } };

            this.ShouldReturnExpectedBoundary(xaml, expected);
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
+ Environment.NewLine + "        <Grid Grid.Row=\"0\">"
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
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>";

            var expected = new Dictionary<int, int>() { { 199, 672 }, { 743, 1036 } };

            this.ShouldReturnExpectedBoundary(xaml, expected);
        }

        private void ShouldReturnExpectedBoundary(string xaml, Dictionary<int, int> expected)
        {
            var exclusions = InsertRowDefinitionAction.GetExclusions(xaml);

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
