// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.Grid
{
    [TestClass]
    public class MissingDefinitionsTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void Row_0_NotReported_IfNoDefinitions()
        {
            var xaml = @"<Grid>
	<TextBlock Text=""hello world"" Grid.Row=""0"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Row_0_NotReported_IfEmptyDefinitionGroup()
        {
            var xaml = @"<Grid>
	<Grid.RowDefinitions />

	<TextBlock Text=""hello world"" Grid.Row=""0"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Column_0_NotReported_IfNoDefinitions()
        {
            var xaml = @"<Grid>
	<TextBlock Text=""hello world"" Grid.Column=""0"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Column_0_NotReported_IfEmptyDefinitionGroup()
        {
            var xaml = @"<Grid>
	<Grid.ColumnDefinitions />

	<TextBlock Text=""hello world"" Grid.Column=""0"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void RowAssignmentInRoot_NotReportedIfNoOwnDefinitions()
        {
            var xaml = @"<Grid Grid.Row=""2"">
	<TextBlock Text=""hello world"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void ColumnAssignmentInRoot_NotReportedIfOnOwnDefinitions()
        {
            var xaml = @"<Grid Grid.Column=""2"">
	<TextBlock Text=""hello world"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            // This is zero because only looking at the outer grid.
            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
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

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void NestedGrid_DefinitionInInnerNotOuter_OtherInner_Short_FindInvalid()
        {
            var xaml = @"<Grid RowDefinitions=""*,*,*,*"">

     <Grid Grid.Row=""1"" />

     <Grid Grid.Row=""5"" RowDefinitions=""*,*,*,*,*,*,*,*,*"">
        <TextBlock Text=""hello world"" />
    </Grid>
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.ReplaceAttributeValue));
            Assert.AreEqual(Attributes.RowDefinitions, actual[0].Name);
            Assert.AreEqual("*,*,*,*,*,*", actual[0].Value);
        }

        [TestMethod]
        public void ConciseSyntaxWithMultipleChildGrids_ColumnDefinitions()
        {
            var xaml = @"<Grid ColumnDefinitions=""*,*"">
    <Grid ColumnDefinitions=""*,*"" />
    <Grid
        ColumnDefinitions=""*,*,*,*,*"">
        <Button Grid.Column=""3"" />
        <Button Grid.Column=""4"" />
    </Grid>
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void ConciseSyntaxWithMultipleChildGrids_RowDefinitions()
        {
            var xaml = @"<Grid RowDefinitions=""*,*"">
    <Grid RowDefinitions=""*,*"" />
    <Grid
      RowDefinitions=""*,*,*,*,*"">
        <Button Grid.Row=""3"" />
        <Button Grid.Row=""4"" />
    </Grid>
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }
    }
}
