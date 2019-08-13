// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
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

            var sut = new GridProcessor(ProjectType.Any, new DefaultTestLogger());

            var snapshot = new FakeTextSnapshot();

            sut.Process("testfile.xaml", 1, xaml, "	    ", snapshot, outputTags);

            Assert.AreEqual(0, outputTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Real_ProfileConfigControl_GridProcessor()
        {
            var xaml = System.IO.File.ReadAllText("./Misc/ProfileConfigControl.xaml");

            var outputTags = new TagList();

            var sut = new GridProcessor(ProjectType.Wpf, new DefaultTestLogger());

            var snapshot = new FakeTextSnapshot();

            sut.Process("testfile.xaml", 1, xaml, "    ", snapshot, outputTags);

            Assert.AreEqual(0, outputTags.OfType<MissingRowDefinitionTag>().Count());
        }
    }
}
