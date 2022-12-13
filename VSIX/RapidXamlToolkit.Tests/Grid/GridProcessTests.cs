// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Tests.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.Grid
{
    [TestClass]
    public class GridProcessTests
    {
        // TODO: review this test
        // This should detect for and return as many tags as possible
        [TestMethod]
        public void CreatesMultipleTags()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition />
        <RowDefinition />
    </Grid.RowDefinitions>
</Grid>";

            var outputTags = new TagList();

            var sut = new GridProcessor(new ProcessorEssentialsForSimpleTests());

            var snapshot = new FakeTextSnapshot(xaml.Length);

            sut.Process("testfile.xaml", 0, xaml, string.Empty, snapshot, outputTags);

            Assert.AreEqual(2, outputTags.Count);
            Assert.AreEqual(2, outputTags.OfType<InsertRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void CreateNoDefinitionTags_IfJustHaveRowsAndColumns_RowsFirst()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions />
    <Grid.ColumnDefinitions />

    <!-- Grid contents -->
</Grid>";

            var outputTags = new TagList();

            var sut = new GridProcessor(new ProcessorEssentialsForSimpleTests());

            var snapshot = new FakeTextSnapshot(xaml.Length);

            sut.Process("testfile.xaml", 0, xaml, string.Empty, snapshot, outputTags);

            Assert.AreEqual(0, outputTags.Count);
        }

        [TestMethod]
        public void CreateNoDefinitionTags_IfJustHaveRowsAndColumns_ColsFirst()
        {
            var xaml = @"<Grid>
    <Grid.ColumnDefinitions />
    <Grid.RowDefinitions />

    <!-- Grid contents -->
</Grid>";

            var outputTags = new TagList();

            var sut = new GridProcessor(new ProcessorEssentialsForSimpleTests());

            var snapshot = new FakeTextSnapshot(xaml.Length);

            sut.Process("testfile.xaml", 0, xaml, string.Empty, snapshot, outputTags);

            Assert.AreEqual(0, outputTags.Count);
        }
    }
}
