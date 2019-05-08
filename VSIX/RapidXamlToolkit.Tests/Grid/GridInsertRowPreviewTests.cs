// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.Grid
{
    [TestClass]
    public class GridInsertRowPreviewTests
    {
        [TestMethod]
        public void CreatePreviewCorrectly_Row0()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"XXX\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot(), "testfile.xaml")
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(
                            original.Replace("☆", string.Empty),
                            InsertRowDefinitionAction.GetReplacements(1, 3),
                            null,
                            tag);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreatePreviewCorrectly_Row1()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            ☆<RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"XXX\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot(), "testfile.xaml")
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(
                            original.Replace("☆", string.Empty),
                            InsertRowDefinitionAction.GetReplacements(1, 3),
                            null,
                            tag);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreatePreviewCorrectly_Row2()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"XXX\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot(), "testfile.xaml")
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(
                            original.Replace("☆", string.Empty),
                            InsertRowDefinitionAction.GetReplacements(1, 3),
                            null,
                            tag);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreatePreviewCorrectly_WithNestedGrid()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <Grid Grid.Row=\"1\">"
   + Environment.NewLine + "            <!-- content -->"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"XXX\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <Grid Grid.Row=\"2\">"
   + Environment.NewLine + "            <!-- content -->"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot(), "testfile.xaml")
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(
                            original.Replace("☆", string.Empty),
                            InsertRowDefinitionAction.GetReplacements(1, 3),
                            null,
                            tag);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreatePreviewCorrectly_WithNestedGrid_MultipleIdenticalRows()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <Grid Grid.Row=\"1\">"
   + Environment.NewLine + "            <!-- content -->"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        <TextBlock Text=\"OtherFooter\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"XXX\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <Grid Grid.Row=\"2\">"
   + Environment.NewLine + "            <!-- content -->"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "        <TextBlock Text=\"OtherFooter\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot(), "testfile.xaml")
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(
                            original.Replace("☆", string.Empty),
                            InsertRowDefinitionAction.GetReplacements(1, 3),
                            null,
                            tag);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreatePreviewCorrectly_WithNestedGrid_AndExclusions()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <Grid Grid.Row=\"1\">"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + ""
   + Environment.NewLine + "            <TextBlock Tex=\"Excluded\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"XXX\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <Grid Grid.Row=\"2\">"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + "            <RowDefinition Height=\"*\" />"
   + Environment.NewLine + ""
   + Environment.NewLine + "            <TextBlock Tex=\"Excluded\" Grid.Row=\"2\" />"
   + Environment.NewLine + "        </Grid>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot(), "testfile.xaml")
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actualXaml = original.Replace("☆", string.Empty);

            // Get the position of the first grid and use it to find exclusions
            var exclusionGridPos = actualXaml.IndexOf("<Grid>", StringComparison.Ordinal);
            var exclusions = InsertRowDefinitionAction.GetExclusions(actualXaml.Substring(exclusionGridPos));

            var actual = InsertRowDefinitionAction.GetPreviewText(
                            actualXaml,
                            InsertRowDefinitionAction.GetReplacements(1, 3),
                            exclusions,
                            tag);

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RealWorldExample()
        {
            var original = ""
+ Environment.NewLine + "<Window"
+ Environment.NewLine + "    x:Class=\"RxtWpfDemo.MainWindow\""
+ Environment.NewLine + "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""
+ Environment.NewLine + "    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""
+ Environment.NewLine + "    xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\""
+ Environment.NewLine + "    xmlns:local=\"clr-namespace:RxtWpfDemo\""
+ Environment.NewLine + "    xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\""
+ Environment.NewLine + "    Title=\"MainWindow\""
+ Environment.NewLine + "    Width=\"800\""
+ Environment.NewLine + "    Height=\"450\""
+ Environment.NewLine + "    mc:Ignorable=\"d\">"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "            ☆<RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <TextBlock Text=\"hello\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <TextBlock Text=\"world\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + "        <TextBlock Grid.Row=\"2\" Text=\"line 3\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <TextBlock Text=\"hello world\" />"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Button Grid.Row=\"1\" Content=\"click here\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Window>";

            var expected = ""
+ Environment.NewLine + "<Window"
+ Environment.NewLine + "    x:Class=\"RxtWpfDemo.MainWindow\""
+ Environment.NewLine + "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""
+ Environment.NewLine + "    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""
+ Environment.NewLine + "    xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\""
+ Environment.NewLine + "    xmlns:local=\"clr-namespace:RxtWpfDemo\""
+ Environment.NewLine + "    xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\""
+ Environment.NewLine + "    Title=\"MainWindow\""
+ Environment.NewLine + "    Width=\"800\""
+ Environment.NewLine + "    Height=\"450\""
+ Environment.NewLine + "    mc:Ignorable=\"d\">"
+ Environment.NewLine + "    <Grid>"
+ Environment.NewLine + "        <Grid.RowDefinitions>"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "            <RowDefinition Height=\"XXX\" />"
+ Environment.NewLine + "            <RowDefinition Height=\"*\" />"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "            <RowDefinition />"
+ Environment.NewLine + "        </Grid.RowDefinitions>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <TextBlock Text=\"hello\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <TextBlock Text=\"world\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + "        <TextBlock Grid.Row=\"3\" Text=\"line 3\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Grid>"
+ Environment.NewLine + "            <TextBlock Text=\"hello world\" />"
+ Environment.NewLine + "        </Grid>"
+ Environment.NewLine + ""
+ Environment.NewLine + "        <Button Grid.Row=\"2\" Content=\"click here\" />"
+ Environment.NewLine + ""
+ Environment.NewLine + "    </Grid>"
+ Environment.NewLine + "</Window>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot(), "testfile.xaml")
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 451,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(
                            original.Replace("☆", string.Empty),
                            InsertRowDefinitionAction.GetReplacements(1, 5),
                            null,
                            tag);

            StringAssert.AreEqual(expected, actual);
        }
    }
}
