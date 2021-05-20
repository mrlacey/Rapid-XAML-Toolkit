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
    public class MissingDefinitionsTests
    {
        [TestMethod]
        public void Row_0_NotReported_IfNoDefinitions()
        {
            var xaml = @"<Grid>
	<TextBlock Text=""hello world"" Grid.Row=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Row_0_NotReported_IfEmptyDefinitionGroup()
        {
            var xaml = @"<Grid>
	<Grid.RowDefinitions />

	<TextBlock Text=""hello world"" Grid.Row=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Row_0_NotReported_IfOneDefinition()
        {
            var xaml = @"<Grid>
	<Grid.RowDefinitions>
        <RowDefinition Height=""*"" />
	<Grid.RowDefinitions>

	<TextBlock Text=""hello world"" Grid.Row=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Row_0_NotReported_IfTwoDefinitions()
        {
            var xaml = @"<Grid>
	<Grid.RowDefinitions>
        <RowDefinition Height=""*"" />
        <RowDefinition Height=""*"" />
	<Grid.RowDefinitions>

	<TextBlock Text=""hello world"" Grid.Row=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Column_0_NotReported_IfNoDefinitions()
        {
            var xaml = @"<Grid>
	<TextBlock Text=""hello world"" Grid.Column=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingColumnDefinitionTag>().Count());
        }

        [TestMethod]
        public void Column_0_NotReported_IfEmptyDefinitionGroup()
        {
            var xaml = @"<Grid>
	<Grid.ColumnDefinitions />

	<TextBlock Text=""hello world"" Grid.Column=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingColumnDefinitionTag>().Count());
        }

        [TestMethod]
        public void Column_0_NotReported_IfOneDefinition()
        {
            var xaml = @"<Grid>
	<Grid.ColumnDefinitions>
        <ColumnDefinition Width=""*"" />
	<Grid.ColumnDefinitions>

	<TextBlock Text=""hello world"" Grid.Column=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingColumnDefinitionTag>().Count());
        }

        [TestMethod]
        public void Column_0_NotReported_IfTwoDefinitions()
        {
            var xaml = @"<Grid>
	<Grid.ColumnDefinitions>
        <ColumnDefinition Width=""*"" />
        <ColumnDefinition Width=""*"" />
	<Grid.RowDefinitions>

	<TextBlock Text=""hello world"" Grid.Column=""0"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingColumnDefinitionTag>().Count());
        }

        [TestMethod]
        public void RowAssignmentInRoot_NotReportedIfNoOwnDefinitions()
        {
            var xaml = @"<Grid Grid.Row=""2"">
	<TextBlock Text=""hello world"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void ColumnAssignmentInRoot_NotReportedIfNoOwnDefinitions()
        {
            var xaml = @"<Grid Grid.Column=""2"">
	<TextBlock Text=""hello world"" />
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingColumnDefinitionTag>().Count());
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_Expanded()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""*"" />
        <RowDefinition Height=""*"" />
    </Grid.RowDefinitions>

     <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height=""*"" />
            <RowDefinition Height=""*"" />
            <RowDefinition Height=""*"" />
            <RowDefinition Height=""*"" />
        </Grid.RowDefinitions>
        <TextBlock Grid.Row=""3"" Text=""hello world"" />
    </Grid>
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_Short()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""*"" />
        <RowDefinition Height=""*"" />
    </Grid.RowDefinitions>

     <Grid RowDefinitions=""*,*,*,*"">
        <TextBlock Grid.Row=""3"" Text=""hello world"" />
    </Grid>
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_LongOuterThenShort()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""*"" />
        <RowDefinition Height=""*"" />
    </Grid.RowDefinitions>

     <Grid RowDefinitions=""*,*,*,*"">
        <TextBlock Grid.Row=""3"" Text=""hello world"" />
    </Grid>
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_LongOuterThenShort_FindInvalid()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""*"" />
        <RowDefinition Height=""*"" />
        <RowDefinition Height=""*"" />
        <RowDefinition Height=""*"" />
    </Grid.RowDefinitions>

     <Grid RowDefinitions=""*,*"">
        <TextBlock Grid.Row=""3"" Text=""hello world"" />
    </Grid>
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            // This is zero because only looking at the outer grid.
            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_ShortOuterLongerInner()
        {
            var xaml = @"<Grid RowDefinitions=""*,*"">

     <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height=""*"" />
            <RowDefinition Height=""*"" />
            <RowDefinition Height=""*"" />
            <RowDefinition Height=""*"" />
        </Grid.RowDefinitions>
        <TextBlock Grid.Row=""3"" Text=""hello world"" />
    </Grid>
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_OtherInner_Short()
        {
            var xaml = @"<Grid RowDefinitions=""*,*"">

     <Grid />

     <Grid RowDefinitions=""*,*,*,*"">
        <TextBlock Grid.Row=""3"" Text=""hello world"" />
    </Grid>
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(0, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_OtherInner_Short_FindInvalid()
        {
            var xaml = @"<Grid RowDefinitions=""*,*,*,*"">

     <Grid Grid.Row=""1"" />

     <Grid Grid.Row=""5"" RowDefinitions=""*,*"">
        <TextBlock Text=""hello world"" />
    </Grid>
</Grid>";

            var actualTags = this.ProcessGrid(xaml);

            Assert.AreEqual(1, actualTags.OfType<MissingRowDefinitionTag>().Count());
        }

        private List<IRapidXamlAdornmentTag> ProcessGrid(string xaml)
        {
            var outputTags = new TagList();

            var sut = new GridProcessor(new ProcessorEssentialsForSimpleTests());

            var snapshot = new FakeTextSnapshot(xaml.Length);

            sut.Process("testfile.xaml", 1, xaml, "	    ", snapshot, outputTags);

            return outputTags;
        }
    }
}
