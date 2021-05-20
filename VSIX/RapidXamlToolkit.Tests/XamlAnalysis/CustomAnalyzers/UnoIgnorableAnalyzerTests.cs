// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.XamlAnalysis.CustomAnalysis;

namespace RapidXamlToolkit.Tests.XamlAnalysis.CustomAnalyzers
{
    [TestClass]
    public class UnoIgnorableAnalyzerTests
    {
        [TestMethod]
        public void CreateTag_ForXamarin_WhenNotInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:xamarin=""http:/uno.ui/xamarin""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DontCreateTag_ForXamarin_WhenInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:xamarin=""http:/uno.ui/xamarin""
    mc:Ignorable=""d xamarin"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void CreateTag_ForNotWin_WhenNotInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:not_win=""http:/uno.ui/not_win""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DontCreateTag_ForNotWin_WhenInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:not_win=""http:/uno.ui/not_win""
    mc:Ignorable=""d not_win"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void CreateTag_ForAndroid_WhenNotInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:android=""http:/uno.ui/android""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DontCreateTag_ForAndroid_WhenInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:android=""http:/uno.ui/android""
    mc:Ignorable=""d android"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void CreateTag_ForIos_WhenNotInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:ios=""http:/uno.ui/ios""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DontCreateTag_ForIos_WhenInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:ios=""http:/uno.ui/ios""
    mc:Ignorable=""d ios"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void CreateTag_ForWasm_WhenNotInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:wasm=""http:/uno.ui/wasm""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DontCreateTag_ForWasm_WhenInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:wasm=""http:/uno.ui/wasm""
    mc:Ignorable=""d wasm"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void CreateTag_ForForMacos_WhenNotInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:macos=""http:/uno.ui/macos""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(1, actual.Actions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actual.Actions[0].Action);
        }

        [TestMethod]
        public void DontCreateTag_ForForMacos_WhenInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:macos=""http:/uno.ui/macos""
    mc:Ignorable=""d macos"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void CreateTag_ForAll_WhenNotInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:xamarin=""http:/uno.ui/xamarin""
    xmlns:not_win=""http:/uno.ui/not_win""
    xmlns:android=""http:/uno.ui/android""
    xmlns:ios=""http:/uno.ui/ios""
    xmlns:wasm=""http:/uno.ui/wasm""
    xmlns:macos=""http:/uno.ui/macos""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(6, actual.Actions.Count);
        }

        [TestMethod]
        public void DontCreateTag_ForAll_WhenInIgnorableList()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:xamarin=""http:/uno.ui/xamarin""
    xmlns:not_win=""http:/uno.ui/not_win""
    xmlns:android=""http:/uno.ui/android""
    xmlns:ios=""http:/uno.ui/ios""
    xmlns:wasm=""http:/uno.ui/wasm""
    xmlns:macos=""http:/uno.ui/macos""
    mc:Ignorable=""d xamarin not_win android ios wasm macos"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }

        [TestMethod]
        public void DoNothingWhenNoRelevantXmlnsUsed()
        {
            var xaml = @"<Page
    x:Class=""App193.MainPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:App193""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
		<TextBlock Text=""Hello, world!"" Margin=""20"" FontSize=""30"" />
    </Grid>
</Page>";

            var rxElement = CustomAnalysisTestHelper.StringToElement(xaml);

            var sut = new UnoIgnorablesAnalyzer(new TestVisualStudioAbstraction());

            var actual = sut.Analyze(rxElement, FakeExtraAnalysisDetails.Create(ProjectFramework.Uwp));

            Assert.AreEqual(0, actual.Actions.Count);
        }
    }
}
