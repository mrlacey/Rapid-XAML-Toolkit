// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Grid
{
    [TestClass]
    public class InsertRowDefinitionTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CmdEnabledInFirstRowDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldEnableCommand(xaml, true);
        }

        [TestMethod]
        public void CmdEnabledInSecondRowDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldEnableCommand(xaml, true);
        }

        [TestMethod]
        public void CmdNotEnabledBeforeFirstRowDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>☆"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldEnableCommand(xaml, false);
        }

        [TestMethod]
        public void CmdNotEnabledAfterDefinitions()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + "☆"
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldEnableCommand(xaml, false);
        }

        [TestMethod]
        public void GetRowNumberForFirstRowDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <☆RowDefinition Height=\"Auto\" /☆>"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.EachPositionBetweenStarsShouldReturnRowNumber(xaml, 0);
        }

        [TestMethod]
        public void GetRowNumberForSecondRowDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <☆RowDefinition Height=\"*\" /☆>"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.EachPositionBetweenStarsShouldReturnRowNumber(xaml, 1);
        }

        [TestMethod]
        public void GetRowNumberForThirdRowDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <☆RowDefinition Height=\"*\" /☆>"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.EachPositionBetweenStarsShouldReturnRowNumber(xaml, 2);
        }

        [TestMethod]
        public void GetRowNumberForFirstRowDefinitionInNestedGrid()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <☆RowDefinition Height=\"Auto\" /☆>"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.EachPositionBetweenStarsShouldReturnRowNumber(xaml, 0);
        }

        [TestMethod]
        public void GetExpectedReplacementsFromFirstDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

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

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, expected);
        }

        [TestMethod]
        public void GetExpectedReplacementsFromMiddleDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"6\"", " Grid.Row=\"7\""),
                (" Grid.Row=\"5\"", " Grid.Row=\"6\""),
                (" Grid.Row=\"4\"", " Grid.Row=\"5\""),
                (" Grid.Row=\"3\"", " Grid.Row=\"4\""),
                (" Grid.Row=\"2\"", " Grid.Row=\"3\""),
                (" Grid.Row=\"1\"", " Grid.Row=\"2\""),
            };

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, expected);
        }

        [TestMethod]
        public void GetExpectedReplacementsFromLastDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"6\"", " Grid.Row=\"7\""),
            };

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, expected);
        }

        [TestMethod]
        public void GetExpectedReplacementsIgnoresSubsequentNestedGrids()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
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
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"2\"", " Grid.Row=\"3\""),
                (" Grid.Row=\"1\"", " Grid.Row=\"2\""),
            };

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, expected);
        }

        [TestMethod]
        public void GetExpectedReplacementsInNestedGrid()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"2\">"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition☆ Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = new List<(string, string)>
            {
                (" Grid.Row=\"1\"", " Grid.Row=\"2\""),
            };

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_Auto()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"Auto\" />", 57 + (Environment.NewLine.Length * 4)); // 4 = lines

            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_Star()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"*\" />", 100 + (Environment.NewLine.Length * 5)); // 5 = number of rows above

            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_InElementName()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <Row☆Definition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"*\" />", 100 + (Environment.NewLine.Length * 5)); // 5 = number of rows above

            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_WorksForAnyPositionInLine()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />☆"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"*\" />", 100 + (Environment.NewLine.Length * 5)); // 5 = number of rows above

            this.EachPositionBetweenStarsShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_WorksForAnyPositionInLine_FirstLine()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />☆"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"*\" />", 57 + (Environment.NewLine.Length * 4)); // 4 = number of rows above

            this.EachPositionBetweenStarsShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_WorksForAnyPositionInLine_LastLine()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />☆"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"*\" />", 143 + (Environment.NewLine.Length * 6)); // 6 = number of rows above

            this.EachPositionBetweenStarsShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_AtEndOfDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />☆"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"Auto\" />", 57 + (Environment.NewLine.Length * 4));

            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetDefinitionAtCursor_AtStartOfDefinition()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = ("<RowDefinition Height=\"*\" />", 100 + (Environment.NewLine.Length * 5));

            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void GetGridBoundary_NoOtherGrids()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />☆"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var gridStartPos = 10 + (Environment.NewLine.Length * 2);
            var expected = (gridStartPos, 259, new Dictionary<int, int>());

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void GetGridBoundary_OtherGridsBeforeAndAfter()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <StackPanel>"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                ☆<RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />☆"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + "    </StackPanel>"
+ Environment.NewLine + "</Page>";

            var gridStartPos = 297 + (Environment.NewLine.Length * 13); // adjust for number of previous lines in file
            var expected = (gridStartPos, 596, new Dictionary<int, int>());

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void GetGridBoundary_IgnoreSingleNestedGrid()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />☆"
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
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var gridStartPos = 10 + (Environment.NewLine.Length * 2);
            var expected = (gridStartPos, 562, new Dictionary<int, int> { { 263, 549 } });

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void GetGridBoundary_IgnoreMultipleNestedGrids()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />☆"
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
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var gridStartPos = 10 + (Environment.NewLine.Length * 2);
            var expected = (gridStartPos, 856, new Dictionary<int, int> { { 263, 549 }, { 568, 805 } });

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void GetGridBoundary_InNestedGrid()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
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
+ Environment.NewLine + "            <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid Grid.Row=\"1\">"
+ Environment.NewLine + "            <Grid.RowDefinitions>"
+ Environment.NewLine + "                ☆<RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"Auto\" />"
+ Environment.NewLine + "                <RowDefinition Height=\"*\" />☆"
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
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var gridStartPos = 474 + (Environment.NewLine.Length * 21); // lines above in file
            var expected = (gridStartPos, 802, new Dictionary<int, int>());

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinition_GetDefinitionAtCursor()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" /"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = (string.Empty, -1);
            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinition_GetReplacements()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" /"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, null);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinition_GetGridBoundary()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <☆RowDefinition Height=\"Auto☆\""
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = (-1, -1, new Dictionary<int, int>());

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinition_GetRowNumber()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" /"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldReturnRowNumber(xaml, -1);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinition_ShouldEnableCommand()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\""
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldEnableCommand(xaml, false);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinitions_GetDefinitionAtCursor()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefin"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = (string.Empty, -1);
            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinitions_GetReplacements()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDef"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, null);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinitions_GetGridBoundary()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <☆RowDefinition Height=\"Auto\" /☆>"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Gri"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            var expected = (-1, -1, new Dictionary<int, int>());

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinitions_GetRowNumber()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefi"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldReturnRowNumber(xaml, -1);
        }

        [TestMethod]
        public void HandleNoClosingRowDefinitions_ShouldEnableCommand()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDe"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldEnableCommand(xaml, false);
        }

        [TestMethod]
        public void HandleNoClosingGrid_GetDefinitionAtCursor()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Gri"
+ Environment.NewLine + "</Page>";

            var expected = (string.Empty, -1);
            this.PositionAtStarShouldReturnExpectedDefinition(xaml, expected);
        }

        [TestMethod]
        public void HandleNoClosingGrid_GetReplacements()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Gri"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldReturnExpectedReplacements(xaml, null);
        }

        [TestMethod]
        public void HandleNoClosingGrid_GetGridBoundary()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <☆RowDefinition Height=\"Auto\" /☆>"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Gri"
+ Environment.NewLine + "</Page>";

            var expected = (-1, -1, new Dictionary<int, int>());

            this.EachPositionBetweenStarsShouldReturnExpectedBoundary(xaml, expected);
        }

        [TestMethod]
        public void HandleNoClosingGrid_GetRowNumber()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Gri"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldReturnRowNumber(xaml, -1);
        }

        [TestMethod]
        public void HandleNoClosingGrid_ShouldEnableCommand()
        {
            var xaml = ""
+ Environment.NewLine + "<Page>"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition ☆Height=\"Auto\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <!-- Content omitted -->"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Gri"
+ Environment.NewLine + "</Page>";

            this.PositionAtStarShouldEnableCommand(xaml, false);
        }

        private InsertGridRowDefinitionCommandLogic SetUpLogic(string xaml)
        {
            var pos = xaml.IndexOf("☆", StringComparison.Ordinal);

            var vsa = new TestVisualStudioAbstraction
            {
                ActiveDocumentText = xaml.Replace("☆", string.Empty),
                CursorPosition = pos,
            };

            return new InsertGridRowDefinitionCommandLogic(DefaultTestLogger.Create(), vsa);
        }

        private void PositionAtStarShouldEnableCommand(string xaml, bool expected)
        {
            var logic = this.SetUpLogic(xaml);

            var actual = logic.ShouldEnableCommand();

            Assert.AreEqual(expected, actual);
        }

        private void EachPositionBetweenStarsShouldReturnRowNumber(string xaml, int expected)
        {
            var startPos = xaml.IndexOf("☆", StringComparison.Ordinal);
            var endPos = xaml.LastIndexOf("☆", StringComparison.Ordinal) - 1;

            var positionsTested = 0;

            var plainXaml = xaml.Replace("☆", string.Empty);

            for (var pos = startPos; pos < endPos; pos++)
            {
                var vsa = new TestVisualStudioAbstraction
                {
                    ActiveDocumentText = plainXaml,
                    CursorPosition = pos,
                };

                var logic = new InsertGridRowDefinitionCommandLogic(DefaultTestLogger.Create(), vsa);

                var actual = logic.GetRowNumber();

                Assert.AreEqual(expected, actual, $"Failure at {pos} ({startPos}-{endPos})");
            }

            this.TestContext.WriteLine($"{positionsTested} different positions tested.");
        }

        private void PositionAtStarShouldReturnRowNumber(string xaml, int expected)
        {
            var logic = this.SetUpLogic(xaml);

            var actual = logic.GetRowNumber();

            Assert.AreEqual(expected, actual);
        }

        private void PositionAtStarShouldReturnExpectedReplacements(string xaml, List<(string, string)> expected)
        {
            var logic = this.SetUpLogic(xaml);

            var actual = logic.GetReplacements();

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

        private void PositionAtStarShouldReturnExpectedDefinition(string xaml, (string definition, int position) expected)
        {
            var logic = this.SetUpLogic(xaml);

            var (actualDefinition, actualInsertPos) = logic.GetDefinitionAtCursor();

            Assert.AreEqual(expected.definition, actualDefinition);
            Assert.AreEqual(expected.position, actualInsertPos);
        }

        private void EachPositionBetweenStarsShouldReturnExpectedDefinition(string xaml, (string definition, int position) expected)
        {
            var startPos = xaml.IndexOf("☆", StringComparison.Ordinal);
            var endPos = xaml.LastIndexOf("☆", StringComparison.Ordinal) - 1;

            var positionsTested = 0;

            var plainXaml = xaml.Replace("☆", string.Empty);

            for (var pos = startPos; pos < endPos; pos++)
            {
                var vsa = new TestVisualStudioAbstraction
                {
                    ActiveDocumentText = plainXaml,
                    CursorPosition = pos,
                };

                var logic = new InsertGridRowDefinitionCommandLogic(DefaultTestLogger.Create(), vsa);

                var (actualDefinition, actualInsertPos) = logic.GetDefinitionAtCursor();

                Assert.AreEqual(expected.definition, actualDefinition, $"Failure at {pos} ({startPos}-{endPos})");
                Assert.AreEqual(expected.position, actualInsertPos, $"Failure at {pos} ({startPos}-{endPos})");
                positionsTested += 1;
            }

            this.TestContext.WriteLine($"{positionsTested} different positions tested.");
        }

        private void EachPositionBetweenStarsShouldReturnExpectedBoundary(string xaml, (int, int, Dictionary<int, int>) expected)
        {
            var startPos = xaml.IndexOf("☆", StringComparison.Ordinal);
            var endPos = xaml.LastIndexOf("☆", StringComparison.Ordinal) - 1;

            var positionsTested = 0;

            var plainXaml = xaml.Replace("☆", string.Empty);

            for (var pos = startPos; pos < endPos; pos++)
            {
                var vsa = new TestVisualStudioAbstraction
                {
                    ActiveDocumentText = plainXaml,
                    CursorPosition = pos,
                };

                var logic = new InsertGridRowDefinitionCommandLogic(DefaultTestLogger.Create(), vsa);

                var (actualStart, actualEnd, exclusions) = logic.GetGridBoundary();

                Assert.AreEqual(expected.Item1, actualStart, $"Failure at {pos} ({startPos}-{endPos})");
                Assert.AreEqual(expected.Item2, actualEnd, $"Failure at {pos} ({startPos}-{endPos})");
                Assert.IsNotNull(exclusions, $"Failure at {pos} ({startPos}-{endPos})");
                Assert.AreEqual(expected.Item3.Count, exclusions.Count, $"Failure at {pos} ({startPos}-{endPos})");

                foreach (var expectedKey in expected.Item3.Keys)
                {
                    Assert.IsTrue(exclusions.ContainsKey(expectedKey), $"Failure at {pos} , Key {expectedKey}");
                    Assert.AreEqual(exclusions[expectedKey], expected.Item3[expectedKey], $"Failure at {pos} , Value for {expectedKey}");
                }

                positionsTested += 1;
            }

            this.TestContext.WriteLine($"{positionsTested} different positions tested.");
        }
    }
}
