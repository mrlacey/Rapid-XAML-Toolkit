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

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot())
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(original.Replace("☆", string.Empty), InsertRowDefinitionAction.GetReplacements(1, 3), null, tag);

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

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot())
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(original.Replace("☆", string.Empty), InsertRowDefinitionAction.GetReplacements(1, 3), null, tag);

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

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot())
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(original.Replace("☆", string.Empty), InsertRowDefinitionAction.GetReplacements(1, 3), null, tag);

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

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot())
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(original.Replace("☆", string.Empty), InsertRowDefinitionAction.GetReplacements(1, 3), null, tag);

            StringAssert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void RealWorldExample()
        {
            var original = @"
<Window
    x:Class=""RxtWpfDemo.MainWindow""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:local=""clr-namespace:RxtWpfDemo""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    Title=""MainWindow""
    Width=""800""
    Height=""450""
    mc:Ignorable=""d"">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
            ☆<RowDefinition Height=""*"" />
            <RowDefinition />
            <RowDefinition />
            <RowDefinition />
        </Grid.RowDefinitions>

        <TextBlock Text=""hello"" />

        <TextBlock Text=""world"" />


        <TextBlock Grid.Row=""2"" Text=""line 3"" />

        <Grid>
            <TextBlock Text=""gdsgsag"" />
        </Grid>

        <Button Grid.Row=""1"" Content=""click here"" />

    </Grid>
</Window>";

            var expected = @"
<Window
    x:Class=""RxtWpfDemo.MainWindow""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:local=""clr-namespace:RxtWpfDemo""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    Title=""MainWindow""
    Width=""800""
    Height=""450""
    mc:Ignorable=""d"">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
            <RowDefinition Height=""XXX"" />
            <RowDefinition Height=""*"" />
            <RowDefinition />
            <RowDefinition />
            <RowDefinition />
        </Grid.RowDefinitions>

        <TextBlock Text=""hello"" />

        <TextBlock Text=""world"" />


        <TextBlock Grid.Row=""3"" Text=""line 3"" />

        <Grid>
            <TextBlock Text=""gdsgsag"" />
        </Grid>

        <Button Grid.Row=""2"" Content=""click here"" />

    </Grid>
</Window>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot())
            {
                XamlTag = "<RowDefinition Height=\"XXX\" />",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 451,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(original.Replace("☆", string.Empty), InsertRowDefinitionAction.GetReplacements(1, 5), null, tag);

            StringAssert.AreEqual(expected, actual);
        }
    }
}
