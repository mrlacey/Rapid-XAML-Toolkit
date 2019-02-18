// Copyright (c) Microsoft Corporation. All rights reserved.
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

            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new GridProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, string.Empty, snapshot, outputTags);

            Assert.AreEqual(3, outputTags.Count);
            Assert.AreEqual(2, outputTags.OfType<InsertRowDefinitionTag>().Count());
            Assert.AreEqual(1, outputTags.OfType<AddColumnDefinitionsTag>().Count());
        }

        [TestMethod]
        public void CreateAllDefinitionTags()
        {
            var xaml = @"<Grid>
    <!-- Grid contents -->
</Grid>";

            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new GridProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, string.Empty, snapshot, outputTags);

            Assert.AreEqual(3, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<AddRowDefinitionsTag>().Count());
            Assert.AreEqual(1, outputTags.OfType<AddColumnDefinitionsTag>().Count());
            Assert.AreEqual(1, outputTags.OfType<AddRowAndColumnDefinitionsTag>().Count());
        }

        [TestMethod]
        public void CreateRowDefinitionTags_IfJustHaveColumns()
        {
            var xaml = @"<Grid>
    <Grid.ColumnDefinitions />

    <!-- Grid contents -->
</Grid>";

            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new GridProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, string.Empty, snapshot, outputTags);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<AddRowDefinitionsTag>().Count());
        }

        [TestMethod]
        public void CreateColumnDefinitionTags_IfJustHaveRows()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions />

    <!-- Grid contents -->
</Grid>";

            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new GridProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, string.Empty, snapshot, outputTags);

            Assert.AreEqual(1, outputTags.Count);
            Assert.AreEqual(1, outputTags.OfType<AddColumnDefinitionsTag>().Count());
        }

        [TestMethod]
        public void CreateNoDefinitionTags_IfJustHaveRowsAndColumns_RowsFirst()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions />
    <Grid.ColumnDefinitions />

    <!-- Grid contents -->
</Grid>";

            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new GridProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, string.Empty, snapshot, outputTags);

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

            var outputTags = new List<IRapidXamlAdornmentTag>();

            var sut = new GridProcessor();

            var snapshot = new FakeTextSnapshot();

            sut.Process(0, xaml, string.Empty , snapshot, outputTags);

            Assert.AreEqual(0, outputTags.Count);
        }
    }
}
