// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    [TestClass]
    public class GridProcessorTests : AnalyzerTestsBase
    {
        [TestMethod]
        public void MissingRowDefinition_NoDefinitions_Detected()
        {
            var xaml = @"<Grid><TextBlock Grid.Row=""1""></Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.AddAttribute));
            Assert.AreEqual(Attributes.RowDefinitions, actual[0].Name);
            Assert.AreEqual("*,*", actual[0].Value);
        }

        [TestMethod]
        public void MissingRowDefinition_SomeElementDefinitions_Detected()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""Auto"" />
        <RowDefinition Height=""Auto"" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row=""4"">
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            // TODO: Need to support RowDefinitions defined with the element syntax - add a new issue for future tracking/changes (custom analysis v2)
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.HighlightWithoutAction));
            Assert.AreEqual(StringRes.UI_XamlAnalysisMissingRowDefinitionDescription.WithParams(4), actual[0].Description);
        }

        [TestMethod]
        public void MissingColumnDefinition_NoDefinitions_Detected()
        {
            var xaml = @"<Grid><TextBlock Grid.Column=""1""></Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.AddAttribute));
            Assert.AreEqual(Attributes.ColumnDefinitions, actual[0].Name);
            Assert.AreEqual("*,*", actual[0].Value);
        }

        [TestMethod]
        public void MissingColumnDefinition_SomeElementDefinitions_Detected()
        {
            var xaml = @"<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width=""Auto"" />
        <ColumnDefinition Width=""Auto"" />
    </Grid.ColumnDefinitions>

    <TextBlock Grid.Column=""4"">
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            // TODO: Need to support RowDefinitions defined with the element syntax - add a new issue for future tracking/changes (custom analysis v2)
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.HighlightWithoutAction));
            Assert.AreEqual(StringRes.UI_XamlAnalysisMissingColumnDefinitionDescription.WithParams(4), actual[0].Description);
        }

        [TestMethod]
        public void RowSpan_OverFlow_IgnoredIfInComment()
        {
            var xaml = @"<Grid>
    <!--<TextBlock Grid.Row=""1"" Grid.RowSpan=""2"" />-->
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectConcise_RowDefinitions()
        {
            var xaml = @"<Grid RowDefinitions=""*,*"">
    <Label Grid.Row=""1"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectConcise_ColumnDefinitions()
        {
            var xaml = @"<Grid ColumnDefinitions=""*,*"">
    <Label Grid.Column=""1"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DetectOutsideConcise_RowDefinitions()
        {
            var xaml = @"<Grid RowDefinitions=""*,*"">
    <Label Grid.Row=""2"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.ReplaceAttributeValue));
            Assert.AreEqual(Attributes.RowDefinitions, actual[0].Name);
            Assert.AreEqual("*,*,*", actual[0].Value);
        }

        [TestMethod]
        public void DetectOutsideConcise_ColumnDefinitions()
        {
            var xaml = @"<Grid ColumnDefinitions=""*,*"">
    <Label Grid.Column=""2"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.Count(a => a.Action == ActionType.ReplaceAttributeValue));
            Assert.AreEqual(Attributes.ColumnDefinitions, actual[0].Name);
            Assert.AreEqual("*,*,*", actual[0].Value);
        }

        [TestMethod]
        public void MissingColumnDefinition_InComment_NotDetected()
        {
            var xaml = @"<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width=""Auto"" />
        <ColumnDefinition Width=""Auto"" />
    </Grid.ColumnDefinitions>

    <!--<TextBlock Grid.Column=""4"">-->
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void MissingRowDefinition_InComment_NotDetected()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""Auto"" />
        <RowDefinition Height=""Auto"" />
    </Grid.RowDefinitions>
    <!--<TextBlock Grid.Row=""4"">-->
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void RowSpan_OverFlow_Detected()
        {
            var xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""Auto"" />
        <RowDefinition Height=""Auto"" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row=""1"" Grid.RowSpan=""2"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(2, actual.Count(a => a.Action == ActionType.HighlightWithoutAction));
            Assert.AreEqual(1, actual.Count(a => a.Code == "RXT101"));
            Assert.AreEqual(1, actual.Count(a => a.Code == "RXT103"));
        }

        [TestMethod]
        public void ColumnSpan_OverFlow_Detected()
        {
            var xaml = @"<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width=""Auto"" />
        <ColumnDefinition Width=""Auto"" />
    </Grid.ColumnDefinitions>

    <TextBlock Grid.Column=""1"" Grid.ColumnSpan=""2"" />
</Grid>";

            var actual = this.Act<GridAnalyzer>(xaml, ProjectFramework.Uwp);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(2, actual.Count(a => a.Action == ActionType.HighlightWithoutAction));
            Assert.AreEqual(1, actual.Count(a => a.Code == "RXT102"));
            Assert.AreEqual(1, actual.Count(a => a.Code == "RXT104"));
        }
    }
}
