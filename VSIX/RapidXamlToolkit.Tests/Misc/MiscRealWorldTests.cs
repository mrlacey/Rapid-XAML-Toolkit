// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.Misc
{
    [TestClass]
    public class MiscRealWorldTests
    {
        [TestMethod]
        public void NestedSelfClosingElementsDontBreakRowDefinitionDetection()
        {
            var xaml = @"
                        <Grid
                            x:Name=""PaneToggleButtonGrid""
                            Margin=""0,0,0,8""
                            HorizontalAlignment=""Left""
                            VerticalAlignment=""Top""
                            Canvas.ZIndex=""100"">

                            <Grid.RowDefinitions>
                                <RowDefinition Height=""Auto"" />
                                <RowDefinition Height=""Auto"" />
                            </Grid.RowDefinitions>

                            <Grid x:Name=""TogglePaneTopPadding""
                                  Height=""{Binding RelativeSource={RelativeSource TemplatedParent}, Path=TemplateSettings.TopPadding}""/>

                            <Grid x:Name=""ButtonHolderGrid"" Grid.Row=""1"">
                                <Button x:Name=""NavigationViewBackButton""
                                        Style=""{StaticResource NavigationBackButtonNormalStyle}""
                                        VerticalAlignment=""Top""
                                        Visibility=""{Binding RelativeSource={RelativeSource TemplatedParent}, Path=TemplateSettings.BackButtonVisibility}""
                                        IsEnabled=""{TemplateBinding IsBackEnabled}""/>

                                <Button
                                    x:Name=""TogglePaneButton""
                                    Style=""{TemplateBinding PaneToggleButtonStyle}""
                                    AutomationProperties.LandmarkType=""Navigation""
                                    Visibility=""{Binding RelativeSource={RelativeSource TemplatedParent}, Path=TemplateSettings.PaneToggleButtonVisibility}""
                                    VerticalAlignment=""Top""/>
                            </Grid>

                        </Grid>
";

            var outputTags = new TagList();

            var sut = new GridProcessor(new ProcessorEssentialsForSimpleTests());

            var snapshot = new RealTextSnapshot(new FakeTextSnapshot());

            sut.Process("testfile.xaml", 1, xaml, "	    ", snapshot, outputTags);

            Assert.AreEqual(0, outputTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Real_ProfileConfigControl_GridProcessor()
        {
            var xaml = System.IO.File.ReadAllText("./Misc/ProfileConfigControl.xaml");

            var outputTags = new TagList();

            var sut = new GridProcessor(new ProcessorEssentialsForSimpleTests(ProjectType.Wpf));

            var snapshot = new RealTextSnapshot(new FakeTextSnapshot());

            sut.Process("testfile.xaml", 1, xaml, "    ", snapshot, outputTags);

            Assert.AreEqual(0, outputTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Real_Generic()
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(".\\Misc\\Generic.xaml");

            var snapshot = new RealTextSnapshot(new FakeTextSnapshot());

            XamlElementExtractor.Parse(ProjectType.Uwp, "Generic.xaml", snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Uwp, string.Empty, new TestVisualStudioAbstraction(), DefaultTestLogger.Create()), result.Tags, new TestVisualStudioAbstraction());

            Assert.AreEqual(0, result.Tags.OfType<MissingRowDefinitionTag>().Count());
            Assert.AreEqual(0, result.Tags.OfType<MissingColumnDefinitionTag>().Count());
        }

        [TestMethod]
        public void Real_ParseWithoutError_ComboBox()
        {
            this.ParseWithoutError(".\\Misc\\ComboBox.xaml", ProjectType.Wpf);
        }

        private void ParseWithoutError(string filePath, ProjectType projType)
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(filePath);

            var snapshot = new RealTextSnapshot(new FakeTextSnapshot());

            try
            {
                XamlElementExtractor.Parse(
                    projType,
                    Path.GetFileName(filePath),
                    snapshot,
                    text,
                    RapidXamlDocument.GetAllProcessors(projType, string.Empty, new TestVisualStudioAbstraction(), DefaultTestLogger.Create()),
                    result.Tags,
                    new TestVisualStudioAbstraction());
            }
            catch (Exception exc)
            {
                Assert.Fail($"Parsing failed for '{filePath}'{Environment.NewLine}{exc}");
            }
        }
    }
}
