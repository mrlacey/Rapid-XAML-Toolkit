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
        public void EmptyNameDoesNotCauseException()
        {
            var xaml = @"
<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage
    x:Class=""TestNamespace.Views.GenresPage""
    xmlns=""http://xamarin.com/schemas/2014/forms""
    xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
    xmlns:c=""clr-namespace:TestNamespace.Controls""
    x:Name=""""
    Background=""{DynamicResource GsPageBackground}""
    Shell.NavBarIsVisible=""False"">
    <ContentPage.Content>
        <Grid RowDefinitions=""78,*"" ColumnDefinitions=""*,*"">

            <StackLayout Grid.Row=""1"" Margin=""10,0"">
                <c:GsPageTitle Text=""Discover By Genre"" />
                <CollectionView ItemsSource=""{Binding Genres}"">
                    <CollectionView.ItemsLayout>
                        <GridItemsLayout x:Name=""GridLayout"" Orientation=""Vertical"" Span=""2"" HorizontalItemSpacing=""10"" VerticalItemSpacing=""10"" />
                    </CollectionView.ItemsLayout>
                    <CollectionView.ItemTemplate>
                        <DataTemplate>
                            <c:LargeGenreButton GenreName=""{Binding }"" />
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </StackLayout>

            <c:TopNavBar />

        </Grid>
    </ContentPage.Content>
</ContentPage>
";

            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var processors = RapidXamlDocument.GetAllProcessors(ProjectType.Any, string.Empty, vsa, logger);

            var snapshot = new FakeTextSnapshot(xaml.Length);

            var result = new RapidXamlDocument();

            XamlElementExtractor.Parse("SomeFile.xaml", snapshot, xaml, processors, result.Tags, null, null, logger);

            Assert.AreEqual(0, result.Tags.Count());
        }

        [TestMethod]
        public void CommentedElementInsideDottedChildWithAnyMatch()
        {
            var xaml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage
    x:Class=""GigSeekr.Views.ArtistProfilePage""
    xmlns=""http://xamarin.com/schemas/2014/forms""
    xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
    xmlns:c=""clr-namespace:GigSeekr.Controls""
    xmlns:icons=""clr-namespace:GigSeekr.Icons""
    xmlns:tabs=""clr-namespace:Sharpnado.Tabs;assembly=Sharpnado.Tabs""
    xmlns:v=""clr-namespace:GigSeekr.Views""
    xmlns:vm=""clr-namespace:GigSeekr.ViewModels""
    x:Name=""ThePage""
    x:DataType=""vm:ArtistProfileViewModel""
    Background=""{DynamicResource GsPageBackground}""
    Shell.NavBarIsVisible=""False"">
    <ContentPage.Content>
        <c:GridForPageWithNavBar>

            <CollectionView
                x:Name=""TheCollectionView""
                Grid.Row=""1""
                Margin=""0,-5,0,0""
                ItemsSource=""{Binding ThisAsList}"">
                <CollectionView.Header>

                    <Grid x:Name=""HeaderGrid"" RowDefinitions=""250,1,50,20"">
                        <!--
                            The height of BgImage assigned here will be overriden in code
                            to make it square.
                        -->
                        <icons:PlaceholderArtist
                            x:Name=""ImagePlaceholder""
                            Grid.RowSpan=""2""
                            HorizontalOptions=""Center""
                            VerticalOptions=""Center"" />
                        <ActivityIndicator
                            Grid.RowSpan=""2""
                            HorizontalOptions=""Center""
                            IsRunning=""{Binding IsLoading}""
                            IsVisible=""{Binding IsLoading}""
                            VerticalOptions=""Center"" />
                        <c:GsImage
                            x:Name=""BgImage""
                            Grid.RowSpan=""2""
                            FileName=""{Binding SquareImagePath}""
                            HeightRequest=""250""
                            HorizontalOptions=""FillAndExpand""
                            ImageType=""Square"" />
                        <Frame
                            x:Name=""GradientOverlay""
                            Grid.RowSpan=""2""
                            CornerRadius=""0""
                            HasShadow=""False""
                            HeightRequest=""150""
                            HorizontalOptions=""Fill""
                            VerticalOptions=""End"">
                            <Frame.Background>
                                <LinearGradientBrush StartPoint=""0.5,0"" EndPoint=""0.5,1"">
                                    <GradientStop Offset=""0"" Color=""{DynamicResource GsPageBackgroundTransparentColor}"" />
                                    <GradientStop Offset=""1.0"" Color=""{DynamicResource GsPageBackgroundColor}"" />
                                </LinearGradientBrush>
                            </Frame.Background>
                        </Frame>
                        <StackLayout
                            Grid.RowSpan=""2""
                            Margin=""{StaticResource SideMargins}""
                            HorizontalOptions=""StartAndExpand""
                            VerticalOptions=""EndAndExpand"">
                            <c:GsProfileTitle Text=""{Binding Name}"" />
                            <c:GsProfileSubtitle IsVisible=""{Binding HasLocation}"" Text=""{Binding Location}"" />
                            <c:GradientBubbleLabel
                                HorizontalOptions=""Start""
                                IsVisible=""{Binding HasUpcomingEvents}""
                                Text=""{Binding UpcomingSummary}"" />
                            <c:VerticalPadding PaddingHeight=""20"" />
                        </StackLayout>

                        <tabs:TabHostView
                            Grid.Row=""2""
                            Margin=""{StaticResource SideMargins}""
                            SelectedIndex=""{Binding SelectedTabIndex}""
                            SelectedTabIndexChanged=""SelectedTabChanged"">
                            <tabs:TabHostView.Tabs>
                                <c:GsTabItem Label=""About"" />
                                <c:GsTabItem Label=""Events"" />
                                <!--<c:GsTabItem Label=""Music"" />-->
                            </tabs:TabHostView.Tabs>
                        </tabs:TabHostView>
                        <c:VerticalPadding Grid.Row=""3"" PaddingHeight=""20"" />
                    </Grid>
                </CollectionView.Header>
                <CollectionView.ItemTemplate>
                    <v:TabTemplateSelector
                        HeaderTemplate=""{StaticResource GroupedEventSummaryHeader}""
                        ItemTemplate=""{StaticResource GroupedEventSummaryItem}""
                        Tab1DataTypeName=""ArtistProfileViewModel"">
                        <v:TabTemplateSelector.Tab1Template>
                            <DataTemplate>
                                <v:ArtistProfilePageAboutTab BindingContext=""{Binding}"" />
                            </DataTemplate>
                        </v:TabTemplateSelector.Tab1Template>
                    </v:TabTemplateSelector>
                </CollectionView.ItemTemplate>
                <CollectionView.EmptyView>
                    <ContentView HorizontalOptions=""Center"" VerticalOptions=""FillAndExpand"">
                        <Grid
                            Margin=""50,50,50,0""
                            RowDefinitions=""102,Auto""
                            RowSpacing=""25"">
                            <icons:NoEvents HorizontalOptions=""Center"" />
                            <c:NoResultsMessage Grid.Row=""1"" Text=""We don't currently have any events on our system for this artist."" />
                        </Grid>
                    </ContentView>
                </CollectionView.EmptyView>
                <CollectionView.Footer>
                    <c:GsPageBottomPadding />
                </CollectionView.Footer>
            </CollectionView>
   
            <c:TopNavBar>
                <c:GsTopBarIconContainer>
                    <c:TopBarIcon
                        Command=""{Binding AddBookmarkCommand}""
                        Icon=""Bookmark""
                        IsFilled=""{Binding IsBookmarked}"" />
                    <c:TopBarIcon
                        Command=""{Binding AddFavouriteCommand}""
                        Icon=""Favourite""
                        IsFilled=""{Binding IsFavourite}"" />
                    <!--<c:TopBarIcon Icon=""Notify"" Command=""{Binding SetNotificationCommand}"" IsFilled=""{Binding IsInNotifications}"" />-->
                    <!--<c:TopBarIcon Command=""{Binding ShareCommand}"" Icon=""Share"" />-->
                </c:GsTopBarIconContainer>
            </c:TopNavBar>

            <Grid Grid.RowSpan=""2"" IsVisible=""{Binding ShowRemoveBookmarkPopup}"">
                <Grid BackgroundColor=""{DynamicResource GsShellBackgroundColor}"" Opacity=""0.8"" />
                <v:DeleteFromBookmarkPopup />
            </Grid>

        </c:GridForPageWithNavBar>
    </ContentPage.Content>
</ContentPage>
";

            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            var processors = RapidXamlDocument.GetAllProcessors(ProjectType.XamarinForms, "not-a-real.csproj", vsa, logger);

            var snapshot = new FakeTextSnapshot(xaml.Length);

            var result = new RapidXamlDocument();

            // Parse this twice so execute without and with any caching
            XamlElementExtractor.Parse("SomeFile.xaml", snapshot, xaml, processors, result.Tags, null, null, logger);
            XamlElementExtractor.Parse("SomeFile.xaml", snapshot, xaml, processors, result.Tags, null, null, logger);

            Assert.AreEqual(0, result.Tags.Where(t => t.ConfiguredErrorType == TagErrorType.Error).Count());
        }

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

            var snapshot = new FakeTextSnapshot(xaml.Length);

            sut.Process("testfile.xaml", 1, xaml, "	    ", snapshot, outputTags);

            Assert.AreEqual(0, outputTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Real_ProfileConfigControl_GridProcessor()
        {
            var xaml = System.IO.File.ReadAllText("./Misc/ProfileConfigControl.xaml");

            var outputTags = new TagList();

            var sut = new GridProcessor(new ProcessorEssentialsForSimpleTests(ProjectType.Wpf));

            var snapshot = new FakeTextSnapshot(xaml.Length);

            sut.Process("testfile.xaml", 1, xaml, "    ", snapshot, outputTags);

            Assert.AreEqual(0, outputTags.OfType<MissingRowDefinitionTag>().Count());
        }

        [TestMethod]
        public void Real_Generic()
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(".\\Misc\\Generic.xaml");

            var snapshot = new FakeTextSnapshot(text.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            XamlElementExtractor.Parse("Generic.xaml", snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Uwp, string.Empty, vsa, logger), result.Tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Uwp, null, vsa, logger), logger);

            Assert.AreEqual(0, result.Tags.OfType<MissingRowDefinitionTag>().Count());
            Assert.AreEqual(0, result.Tags.OfType<MissingColumnDefinitionTag>().Count());
        }

        [TestMethod]
        public void Real_AsyncRelayCommandPage()
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(".\\Misc\\AsyncRelayCommandPage.xaml");

            var snapshot = new FakeTextSnapshot(text.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            XamlElementExtractor.Parse("AsyncRelayCommandPage.xaml", snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Uwp, string.Empty, vsa, logger), result.Tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.Uwp, null, vsa, logger), logger);

            Assert.IsTrue(true, "Got here without error.");
        }

        [TestMethod]
        public void Real_ParseWithoutError_ComboBox()
        {
            this.ParseWithoutError(".\\Misc\\ComboBox.xaml", ProjectType.Wpf);
        }

        [TestMethod]
        public void Real_ParseWithoutError_XmlSpace1()
        {
            this.ParseWithoutError(".\\Misc\\XmlSpace1.xaml", ProjectType.Wpf);
        }

        [TestMethod]
        public void Real_ParseWithoutError_PageWithXmlEncoding()
        {
            this.ParseWithoutError(".\\Misc\\PageWithXmlEncoding.xaml", ProjectType.Uwp);
        }

        [TestMethod]
        public void Real_ParseWithoutError_WpfPageWithExpandedCheckbox()
        {
            this.ParseWithoutError(".\\Misc\\WpfEmptyCheckBox.xaml", ProjectType.Wpf);
        }

        [TestMethod]
        public void Real_ProfileSearchPage_XamarinForms_NestedGrids_SimpleRowDefinitions()
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(".\\Misc\\ProfileSearchPage.xaml");

            var snapshot = new FakeTextSnapshot(text.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            XamlElementExtractor.Parse("ProfileSearchPage.xaml", snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.XamarinForms, string.Empty, vsa, logger), result.Tags, null, RapidXamlDocument.GetEveryElementProcessor(ProjectType.XamarinForms, null, vsa, logger), logger);

            Assert.AreEqual(0, result.Tags.OfType<MissingRowDefinitionTag>().Count());
            Assert.AreEqual(0, result.Tags.OfType<MissingColumnDefinitionTag>().Count());
        }

        private void ParseWithoutError(string filePath, ProjectType projType)
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(filePath);

            var snapshot = new FakeTextSnapshot(text.Length);
            var vsa = new TestVisualStudioAbstraction();
            var logger = DefaultTestLogger.Create();

            try
            {
                XamlElementExtractor.Parse(
                    Path.GetFileName(filePath),
                    snapshot,
                    text,
                    RapidXamlDocument.GetAllProcessors(projType, string.Empty, vsa, logger),
                    result.Tags,
                    null,
                    RapidXamlDocument.GetEveryElementProcessor(projType, null, vsa, logger),
                    logger);
            }
            catch (Exception exc)
            {
                Assert.Fail($"Parsing failed for '{filePath}'{Environment.NewLine}{exc}");
            }
        }
    }
}
