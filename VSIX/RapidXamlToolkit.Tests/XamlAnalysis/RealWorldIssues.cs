// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;
using RapidXaml.TestHelpers;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    [TestClass]
    public class RealWorldIssues
    {
        [TestMethod]
        public void Issue183()
        {
            var xaml = @"<Grid Margin=""9"">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width=""Auto"" />
                        <ColumnDefinition Width=""*"" />
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height=""Auto"" />
                        <RowDefinition Height=""Auto"" />
                        <RowDefinition Height=""Auto"" />
                        <RowDefinition Height=""Auto"" />
                        <RowDefinition Height=""Auto"" />
                        <RowDefinition Height=""Auto"" />
                        <RowDefinition Height=""*"" />
                        <RowDefinition Height=""Auto"" />
                    </Grid.RowDefinitions>
                    <TextBlock
                        Grid.Row=""1""
                        Grid.Column=""0""
                        Margin=""0,0,10,0""
                        Text=""{x:Static strings:StringRes.Options_ClassGrouping}""
                        ToolTip=""{x:Static strings:StringRes.Options_ClassGroupingDescription}"" />
                    <TextBox
                        Grid.Row=""1""
                        Grid.Column=""1""
                        Margin=""0,0,0,6""
                        AutomationProperties.HelpText=""{x:Static strings:StringRes.Options_ClassGroupingDescription}""
                        AutomationProperties.Name=""{x:Static strings:StringRes.Options_ClassGrouping}""
                        Text=""{Binding ClassGrouping, Mode=TwoWay}"" />

                    <TextBlock
                        Grid.Row=""2""
                        Grid.Column=""0""
                        Margin=""0,0,10,0""
                        Text=""{x:Static strings:StringRes.Options_FallbackOutput}""
                        ToolTip=""{x:Static strings:StringRes.Options_FallbackOutputDescription}"" />
                    <themes:ClassicBorderDecorator
                        x:Name=""FallbackOutputBorder""
                        Grid.Row=""2""
                        Grid.Column=""1""
                        Margin=""0,0,0,6""
                        BorderStyle=""Sunken"">
                        <Grid>
                            <avalonEdit:TextEditor
                                x:Name=""FallbackOutputEntry""
                                MinHeight=""34""
                                AutomationProperties.HelpText=""{x:Static strings:StringRes.Options_FallbackOutputDescription}""
                                AutomationProperties.Name=""{x:Static strings:StringRes.Options_FallbackOutput}""
                                BorderBrush=""{Binding Path=BorderBrush, ElementName=ReferenceTextBox}""
                                BorderThickness=""{Binding Path=BorderThickness, ElementName=ReferenceTextBox}""
                                HorizontalScrollBarVisibility=""Hidden""
                                PreviewKeyDown=""TextEditorPreviewKeyDown""
                                SyntaxHighlighting=""XML""
                                TextChanged=""OnEditorTextChanged""
                                VerticalScrollBarVisibility=""Auto""
                                WordWrap=""True"">
                                <avalonEdit:TextEditor.Options>
                                    <avalonEdit:TextEditorOptions EnableHyperlinks=""False"" />
                                </avalonEdit:TextEditor.Options>
                            </avalonEdit:TextEditor>
                            <local:WarningTriangle
                                x:Name=""FallbackOutputEntryWarning""
                                Margin=""2""
                                HorizontalAlignment=""Right""
                                VerticalAlignment=""Top""
                                Visibility=""Collapsed"" />
                        </Grid>
                    </themes:ClassicBorderDecorator>

                    <TextBlock
                        Grid.Row=""3""
                        Grid.Column=""0""
                        Margin=""0,0,10,0""
                        Text=""{x:Static strings:StringRes.Options_SubPropertyOutput}""
                        ToolTip=""{x:Static strings:StringRes.Options_SubPropertyOutputDescription}"" />
                    <themes:ClassicBorderDecorator
                        x:Name=""SubPropertyOutputBorder""
                        Grid.Row=""3""
                        Grid.Column=""1""
                        Margin=""0,0,0,6""
                        BorderStyle=""Sunken"">
                        <Grid>
                            <avalonEdit:TextEditor
                                x:Name=""SubPropertyOutputEntry""
                                MinHeight=""34""
                                AutomationProperties.HelpText=""{x:Static strings:StringRes.Options_SubPropertyOutputDescription}""
                                AutomationProperties.Name=""{x:Static strings:StringRes.Options_SubPropertyOutput}""
                                BorderBrush=""{Binding Path=BorderBrush, ElementName=ReferenceTextBox}""
                                BorderThickness=""{Binding Path=BorderThickness, ElementName=ReferenceTextBox}""
                                HorizontalScrollBarVisibility=""Hidden""
                                PreviewKeyDown=""TextEditorPreviewKeyDown""
                                SyntaxHighlighting=""XML""
                                TextChanged=""OnEditorTextChanged""
                                VerticalScrollBarVisibility=""Auto""
                                WordWrap=""True"">
                                <avalonEdit:TextEditor.Options>
                                    <avalonEdit:TextEditorOptions EnableHyperlinks=""False"" />
                                </avalonEdit:TextEditor.Options>
                            </avalonEdit:TextEditor>
                            <local:WarningTriangle
                                x:Name=""SubPropertyOutputEntryWarning""
                                Margin=""2""
                                HorizontalAlignment=""Right""
                                VerticalAlignment=""Top""
                                Visibility=""Collapsed"" />
                        </Grid>
                    </themes:ClassicBorderDecorator>

                    <TextBlock
                        Grid.Row=""4""
                        Grid.Column=""0""
                        Margin=""0,0,10,0""
                        Text=""{x:Static strings:StringRes.Options_EnumMappingOutput}""
                        ToolTip=""{x:Static strings:StringRes.Options_EnumMappingOutputDescription}"" />
                    <themes:ClassicBorderDecorator
                        x:Name=""EnumMemberOutputBorder""
                        Grid.Row=""4""
                        Grid.Column=""1""
                        Margin=""0""
                        BorderStyle=""Sunken"">
                        <Grid>
                            <avalonEdit:TextEditor
                                x:Name=""EnumMemberOutputEntry""
                                MinHeight=""34""
                                AutomationProperties.HelpText=""{x:Static strings:StringRes.Options_EnumMappingOutputDescription}""
                                AutomationProperties.Name=""{x:Static strings:StringRes.Options_EnumMappingOutput}""
                                BorderBrush=""{Binding Path=BorderBrush, ElementName=ReferenceTextBox}""
                                BorderThickness=""{Binding Path=BorderThickness, ElementName=ReferenceTextBox}""
                                HorizontalScrollBarVisibility=""Hidden""
                                PreviewKeyDown=""TextEditorPreviewKeyDown""
                                SyntaxHighlighting=""XML""
                                TextChanged=""OnEditorTextChanged""
                                VerticalScrollBarVisibility=""Auto""
                                WordWrap=""True"">
                                <avalonEdit:TextEditor.Options>
                                    <avalonEdit:TextEditorOptions EnableHyperlinks=""False"" />
                                </avalonEdit:TextEditor.Options>
                            </avalonEdit:TextEditor>
                            <local:WarningTriangle
                                x:Name=""EnumMemberOutputEntryWarning""
                                Margin=""2""
                                HorizontalAlignment=""Right""
                                VerticalAlignment=""Top""
                                Visibility=""Collapsed"" />
                        </Grid>
                    </themes:ClassicBorderDecorator>

                    <TextBlock
                        Grid.Row=""5""
                        Grid.Column=""0""
                        ☆Grid.ColumnSpan=""2""
                        Margin=""0,6,0,0""
                        Text=""{x:Static strings:StringRes.Options_MappingsHeader}"" />

                    <Grid Grid.Row=""6"" Grid.ColumnSpan=""2"">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width=""*"" />
                            <ColumnDefinition Width=""Auto"" />
                        </Grid.ColumnDefinitions>
                        <ScrollViewer AutomationProperties.Name=""{x:Static strings:StringRes.Options_MappingsHeader}"">
                            <DataGrid
                                x:Name=""DisplayedMappings""
                                CanUserAddRows=""False""
                                CanUserDeleteRows=""False""
                                CanUserReorderColumns=""False""
                                CanUserResizeColumns=""True""
                                CanUserResizeRows=""False""
                                CanUserSortColumns=""False""
                                IsReadOnly=""True""
                                ItemsSource=""{Binding Mappings}""
                                PreviewKeyDown=""GridPreviewKeyDown""
                                SelectedItem=""{Binding Path=SelectedMapping, Mode=TwoWay}""
                                SelectionMode=""Single""
                                SelectionUnit=""FullRow"" />
                        </ScrollViewer>
                        <StackPanel Grid.Column=""1"" Margin=""6,0,0,0"">
                            <StackPanel.Resources>
                                <Style TargetType=""Button"">
                                    <Setter Property=""Height"" Value=""23"" />
                                    <Setter Property=""Width"" Value=""75"" />
                                    <Setter Property=""Margin"" Value=""0,0,0,6"" />
                                </Style>
                            </StackPanel.Resources>
                            <Button
                                x:Name=""AddMappingButton""
                                Click=""AddClicked""
                                Content=""{x:Static strings:StringRes.Options_ButtonAdd}"" />
                            <Button Click=""CopyClicked"" Content=""{x:Static strings:StringRes.Options_ButtonCopy}"" />
                            <Button Click=""DeleteClicked"" Content=""{x:Static strings:StringRes.Options_ButtonDelete}"" />
                        </StackPanel>
                    </Grid>

                    <Grid
                        Grid.Row=""7""
                        Grid.Column=""0""
                        Grid.ColumnSpan=""2""
                        Margin=""0,4,0,0"">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width=""Auto"" />
                            <ColumnDefinition Width=""*"" />
                        </Grid.ColumnDefinitions>
                        <Grid.RowDefinitions>
                            <RowDefinition Height=""Auto"" />
                            <RowDefinition Height=""Auto"" />
                            <RowDefinition Height=""Auto"" />
                            <RowDefinition Height=""Auto"" />
                        </Grid.RowDefinitions>
                        <TextBlock
                            Grid.Row=""0""
                            Grid.Column=""0""
                            Margin=""0,4,10,0""
                            Text=""{x:Static strings:StringRes.Options_MappingType}""
                            ToolTip=""{x:Static strings:StringRes.Options_MappingTypeDescription}"" />
                        <TextBox
                            Grid.Row=""0""
                            Grid.Column=""1""
                            Margin=""0,4,0,6""
                            AutomationProperties.HelpText=""{x:Static strings:StringRes.Options_MappingTypeDescription}""
                            AutomationProperties.Name=""{x:Static strings:StringRes.Options_MappingType}""
                            Text=""{Binding SelectedMapping.Type, Mode=TwoWay, ValidatesOnDataErrors=True, UpdateSourceTrigger=PropertyChanged}"" />

                        <TextBlock
                            Grid.Row=""1""
                            Grid.Column=""0""
                            Margin=""0,0,10,0""
                            Text=""{x:Static strings:StringRes.Options_MappingNameFilter}""
                            ToolTip=""{x:Static strings:StringRes.Options_MappingNameFilterDescription}"" />
                        <TextBox
                            Grid.Row=""1""
                            Grid.Column=""1""
                            Margin=""0,0,0,6""
                            AutomationProperties.HelpText=""{x:Static strings:StringRes.Options_MappingNameFilterDescription}""
                            AutomationProperties.Name=""{x:Static strings:StringRes.Options_MappingNameFilter}""
                            Text=""{Binding SelectedMapping.NameContains, Mode=TwoWay}"" />

                        <CheckBox
                            Grid.Row=""2""
                            Grid.Column=""1""
                            Margin=""0,0,0,6""
                            IsChecked=""{Binding SelectedMapping.IfReadOnly, Mode=TwoWay}"">
                            <TextBlock Text=""{x:Static strings:StringRes.Options_MappingIfReadOnly}"" ToolTip=""{x:Static strings:StringRes.Options_MappingIfReadOnlyDescription}"" />
                        </CheckBox>

                        <TextBlock
                            Grid.Row=""3""
                            Grid.Column=""0""
                            Margin=""0,0,10,0""
                            Text=""{x:Static strings:StringRes.Options_MappingOutput}""
                            ToolTip=""{x:Static strings:StringRes.Options_MappingOutputDescription}"" />
                        <themes:ClassicBorderDecorator
                            x:Name=""SelectedMappingOutputBorder""
                            Grid.Row=""3""
                            Grid.Column=""1""
                            BorderStyle=""Sunken"">
                            <Grid>
                                <avalonEdit:TextEditor
                                    x:Name=""SelectedMappingOutputEntry""
                                    MinHeight=""34""
                                    AutomationProperties.HelpText=""{x:Static strings:StringRes.Options_MappingOutputDescription}""
                                    AutomationProperties.Name=""{x:Static strings:StringRes.Options_MappingOutput}""
                                    BorderBrush=""{Binding Path=BorderBrush, ElementName=ReferenceTextBox}""
                                    BorderThickness=""{Binding Path=BorderThickness, ElementName=ReferenceTextBox}""
                                    HorizontalScrollBarVisibility=""Hidden""
                                    PreviewKeyDown=""TextEditorPreviewKeyDown""
                                    SyntaxHighlighting=""XML""
                                    TextChanged=""OnSelectedMappingOutputChanged""
                                    VerticalScrollBarVisibility=""Auto""
                                    WordWrap=""True"">
                                    <avalonEdit:TextEditor.Options>
                                        <avalonEdit:TextEditorOptions EnableHyperlinks=""False"" />
                                    </avalonEdit:TextEditor.Options>
                                </avalonEdit:TextEditor>
                                <local:WarningTriangle
                                    x:Name=""SelectedMappingOutputEntryWarning""
                                    Margin=""2""
                                    HorizontalAlignment=""Right""
                                    VerticalAlignment=""Top""
                                    Visibility=""Collapsed"" />
                            </Grid>
                        </themes:ClassicBorderDecorator>
                    </Grid>

                </Grid>";

            var offset = xaml.IndexOf('☆');
            var element = XamlElementProcessor.GetSubElementAtPosition(ProjectType.Wpf, "testFile.xaml", new FakeTextSnapshot(), xaml.Replace("☆", string.Empty), offset, new DefaultTestLogger(), string.Empty, new TestVisualStudioAbstraction());

            Assert.IsNotNull(element);
        }

        [TestMethod]
        public void Issue455()
        {
            var xaml = @"
        <muxc:TwoPaneView
            x:Name=""TwoPaneContent""
            Grid.Column=""1""
            Margin=""40,0,0,0""
            MinTallModeHeight=""Infinity""
            MinWideModeWidth=""Infinity""
            ModeChanged=""OnTwoPaneViewModeChanged"">
            <muxc:TwoPaneView.Pane1>
                <!-- Grid necessary because of ContentFrame1 using deferred loaded -->
                <Grid Margin=""0,32,0,0"">
                    <Frame x:Name=""ContentFrame1"" x:Load=""True"" />
                </Grid>
            </muxc:TwoPaneView.Pane1>
            <muxc:TwoPaneView.Pane2>
                <!-- Grid necessary because of ContentFrame2 using deferred loaded -->
                <Grid>
                    <Frame x:Name=""ContentFrame2"" x:Load=""False"" />
                </Grid>
            </muxc:TwoPaneView.Pane2>
        </muxc:TwoPaneView>";

            var sut = CustomAnalysisTestHelper.StringToElement(xaml);

            var actual = sut.ContainsDescendant("TwoPaneView");

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void FooElement_WithXmlns()
        {
            var xaml = @"<demo:Foo />";

            var expected = RapidXamlElement.Build("demo:Foo");

            var actual = RapidXamlElementExtractor.GetElement(xaml);

            RapidXamlElementAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Grid_RowDefinitions()
        {
            var xaml = @"<Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width=""*"" />
            <ColumnDefinition Width=""50"" />
            <ColumnDefinition Width=""*"" />
        </Grid.ColumnDefinitions>

        <TextBlock Text=""Some content"" />
    </Grid>";

            var sut = RapidXamlElementExtractor.GetElement(xaml);

            Assert.AreEqual(1, sut.Attributes.Count);
            Assert.AreEqual(1, sut.Children.Count);

            var attr = sut.Attributes.First();

            Assert.AreEqual("ColumnDefinitions", attr.Name);
            Assert.IsFalse(attr.HasStringValue);

            Assert.AreEqual(3, attr.Children.Count);
        }
    }
}
