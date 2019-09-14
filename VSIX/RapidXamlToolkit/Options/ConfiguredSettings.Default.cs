// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RapidXamlToolkit.Configuration;

namespace RapidXamlToolkit.Options
{
    public partial class ConfiguredSettings
    {
        public static Settings GetDefaultSettings()
        {
            var config = new RxtSettings();

            var result = new Settings
            {
                FormatVersion = Settings.CurrentFormatVersion,
                ExtendedOutputEnabled = config.ExtendedOutputEnabledByDefault,  // Disable for release builds but want it on by default for testers
                FallBackProfileName = string.Empty,
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        Name = "UWP MVVM Basic StackPanel (no headers)",
                        ProjectType = ProjectType.Uwp,
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForUwpStackPanelWithoutHeaders(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP MVVM Basic StackPanel",
                        ProjectType = ProjectType.Uwp,
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForUwpStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP MVVM Basic 2ColGrid",
                        ProjectType = ProjectType.Uwp,
                        ClassGrouping = "GRID-PLUS-ROWDEFS-2COLS",
                        FallbackOutput = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForUwp2ColGrid(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP MVVM Basic RelativePanel",
                        ProjectType = ProjectType.Uwp,
                        ClassGrouping = "RelativePanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForUwpRelativePanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP MVVM Light StackPanel",
                        ProjectType = ProjectType.Uwp,
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForUwpStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP Prism StackPanel",
                        ProjectType = ProjectType.Uwp,
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForUwpStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "WPF StackPanel",
                        ProjectType = ProjectType.Wpf,
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{Binding Path=$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{Binding $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<ComboBoxItem>$element$</ComboBoxItem>",
                        Mappings = MappingsForWpfStackPanel(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "WPF Grid",
                        ProjectType = ProjectType.Wpf,
                        ClassGrouping = "GRID-PLUS-ROWDEFS",
                        FallbackOutput = "<TextBlock Text=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<ComboBoxItem>$element$</ComboBoxItem>",
                        Mappings = MappingsForWpfGrid(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "WPF 2ColGrid",
                        ProjectType = ProjectType.Wpf,
                        ClassGrouping = "GRID-PLUS-ROWDEFS-2COLS",
                        FallbackOutput = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{Binding Path=$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<ComboBoxItem>$element$</ComboBoxItem>",
                        Mappings = MappingsForWpf2ColGrid(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms StackLayout",
                        ProjectType = ProjectType.XamarinForms,
                        ClassGrouping = "StackLayout",
                        FallbackOutput = "<Label Text=\"{Binding $name$}\" />",
                        SubPropertyOutput = "<Label Text=\"{Binding $name$}\" />",
                        EnumMemberOutput = "<x:String>$elementwithspaces$</x:String>",
                        Mappings = MappingsForXfStackLayout(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms StackLayout with Labels",
                        ProjectType = ProjectType.XamarinForms,
                        ClassGrouping = "StackLayout",
                        FallbackOutput = "<Label Text=\"$namewithspaces$\" /><Label Text=\"{Binding $name$}\" />",
                        SubPropertyOutput = "<Label Text=\"{Binding $name$}\" />",
                        EnumMemberOutput = "<x:String>$elementwithspaces$</x:String>",
                        Mappings = MappingsForXfStackLayoutPlusLabels(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms 2 Col Grid",
                        ProjectType = ProjectType.XamarinForms,
                        ClassGrouping = "GRID-PLUS-ROWDEFS-2COLS",
                        FallbackOutput = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Label Text=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                        SubPropertyOutput = "<Label Text=\"{Binding $name$}\" />",
                        EnumMemberOutput = "<x:String>$elementwithspaces$</x:String>",
                        Mappings = MappingsForXf2ColGrid(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms FlexLayout",
                        ProjectType = ProjectType.XamarinForms,
                        ClassGrouping = "FlexLayout Direction=\"Column\" JustifyContent=\"SpaceEvenly\"",
                        FallbackOutput = "<Label Text=\"{Binding $name$}\" />",
                        SubPropertyOutput = "<Label Text=\"{Binding $name$}\" />",
                        EnumMemberOutput = "<x:String>$elementwithspaces$</x:String>",
                        Mappings = MappingsForXfStackLayout(),
                        AttemptAutomaticDocumentFormatting = true,
                    },
                },
            };

            result.FallBackProfileName = result.Profiles.First().Name;

            var activeUwpDefault = result.Profiles.First(p => p.ProjectType == ProjectType.Uwp);
            result.ActiveProfileNames.Add(activeUwpDefault.ProjectTypeDescription, activeUwpDefault.Name);

            var activeWpfDefault = result.Profiles.First(p => p.ProjectType == ProjectType.Wpf);
            result.ActiveProfileNames.Add(activeWpfDefault.ProjectTypeDescription, activeWpfDefault.Name);

            var activeXfDefault = result.Profiles.First(p => p.ProjectType == ProjectType.XamarinForms);
            result.ActiveProfileNames.Add(activeXfDefault.ProjectTypeDescription, activeXfDefault.Name);

            return result;
        }

        private static ObservableCollection<Mapping> MappingsForUwpStackPanelWithoutHeaders()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBox Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel|cell",
                                Output = "<TextBox InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBox InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBox InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBox InputScope=\"Url\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<AutoSuggestBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Image Source=\"{x:Bind ViewModel.$name$, Mode=OneWay}\" Stretch=\"None\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$safename$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<ToggleSwitch Header=\"$namewithspaces$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$namewithspaces$\" Command=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{x:Bind ViewModel.$name$}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<StackPanel>$members$</StackPanel>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForUwpStackPanelWithHeader()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBox Header=\"$namewithspaces$\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel|cell",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"Url\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<AutoSuggestBox Header=\"$namewithspaces$\" PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Header=\"$namewithspaces$\" Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Image Source=\"{x:Bind ViewModel.$name$, Mode=OneWay}\" Stretch=\"None\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Header=\"$namewithspaces$\" Minimum=\"0\" Maximum=\"100\" x:Name=\"$safename$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Header=\"$namewithspaces$\" Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<ToggleSwitch Header=\"$namewithspaces$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$namewithspaces$\" Command=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView Header=\"$namewithspaces$\" ItemsSource=\"{x:Bind ViewModel.$name$}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<StackPanel>$members$</StackPanel>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForUwpRelativePanelWithHeader()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBox Header=\"$namewithspaces$\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel|cell",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBox Header=\"$namewithspaces$\" InputScope=\"Url\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<AutoSuggestBox Header=\"$namewithspaces$\" PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Header=\"$namewithspaces$\" Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Image Source=\"{x:Bind ViewModel.$name$, Mode=OneWay}\" Stretch=\"None\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Header=\"$namewithspaces$\" Minimum=\"0\" Maximum=\"100\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Header=\"$namewithspaces$\" Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<ToggleSwitch Header=\"$namewithspaces$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$namewithspaces$\" Command=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView Header=\"$namewithspaces$\" ItemsSource=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<StackPanel x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\">$members$</StackPanel>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForUwp2ColGrid()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel|cell",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"Url\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><AutoSuggestBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Image Source=\"{x:Bind ViewModel.$name$, Mode=OneWay}\" Stretch=\"None\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$safename$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ToggleSwitch Header=\"$namewithspaces$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Button Content=\"$namewithspaces$\" Command=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView  Grid.Row=\"$incint$\" Grid.Column=\"0\" Grid.ColumnSpan=\"2\" ItemsSource=\"{x:Bind ViewModel.$name$}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" ></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><StackPanel Grid.Row=\"$repint$\" Grid.Column=\"1\" >$members$</StackPanel>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForWpfStackPanel()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<TextBox Text=\"{Binding Path=$name$}\"$att:MaxLength: MaxLength=\"[1]\"$ />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox$att:MaxLength: MaxLength=\"[1]\"$ />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock><Hyperlink NavigateUri=\"{Binding Path=$name$, Mode=OneWay}\"><Run Text=\"{Binding Path=$name$, Mode=OneWay}\" /></Hyperlink></TextBlock>",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Image Source=\"{Binding Path=$name$, Mode=OneWay}\" Stretch=\"None\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock><Hyperlink NavigateUri=\"{Binding Path=$name$, Mode=OneWay}\"><Run Text=\"{Binding Path=$name$, Mode=OneWay}\" /></Hyperlink></TextBlock>",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$safename$\" Value=\"{Binding Path=$name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker SelectedDate=\"{Binding Path=$name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<CheckBox Content=\"$namewithspaces$\" IsChecked=\"{Binding Path=$name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressBar Name=\"$safename$\" IsIndeterminate=\"{Binding Path=$name$}\" Height=\"20\" Minimum=\"0\" Maximum=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$namewithspaces$\" Command=\"{Binding Path=$name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{Binding Path=$name$}\"><ListView.ItemTemplate><DataTemplate><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{Binding Path=$name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<ComboBox>$members$</ComboBox>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForWpfGrid()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<TextBox Text=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\"$att:MaxLength: MaxLength=\"[1]\"$ />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Grid.Row=\"$incint$\"$att:MaxLength: MaxLength=\"[1]\"$ />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Grid.Row=\"$incint$\"><Hyperlink NavigateUri=\"{Binding Path=$name$, Mode=OneWay}\"><Run Text=\"{Binding Path=$name$, Mode=OneWay}\" /></Hyperlink></TextBlock>",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Image Source=\"{Binding Path=$name$, Mode=OneWay}\" Stretch=\"None\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock Grid.Row=\"$incint$\"><Hyperlink NavigateUri=\"{Binding Path=$name$, Mode=OneWay}\"><Run Text=\"{Binding Path=$name$, Mode=OneWay}\" /></Hyperlink></TextBlock>",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$safename$\" Value=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker SelectedDate=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<CheckBox Content=\"$namewithspaces$\" IsChecked=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressBar Name=\"$safename$\" IsIndeterminate=\"{Binding Path=$name$}\" Height=\"20\" Minimum=\"0\" Maximum=\"1\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$namewithspaces$\" Command=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\"><ListView.ItemTemplate><DataTemplate><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<ComboBox Grid.Row=\"$incint$\">$members$</ComboBox>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForWpf2ColGrid()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox Text=\"{Binding Path=$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\"$att:MaxLength: MaxLength=\"[1]\"$ />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><PasswordBox Grid.Row=\"$repint$\" Grid.Column=\"1\"$att:MaxLength: MaxLength=\"[1]\"$ />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Grid.Row=\"$repint$\" Grid.Column=\"1\"><Hyperlink NavigateUri=\"{Binding Path=$name$, Mode=OneWay}\"><Run Text=\"{Binding Path=$name$, Mode=OneWay}\" /></Hyperlink></TextBlock>",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Image Source=\"{Binding Path=$name$, Mode=OneWay}\" Stretch=\"None\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Grid.Row=\"$repint$\" Grid.Column=\"1\"><Hyperlink NavigateUri=\"{Binding Path=$name$, Mode=OneWay}\"><Run Text=\"{Binding Path=$name$, Mode=OneWay}\" /></Hyperlink></TextBlock>",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$safename$\" Value=\"{Binding Path=$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "date",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><DatePicker SelectedDate=\"{Binding Path=$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><CheckBox Content=\"$namewithspaces$\" IsChecked=\"{Binding Path=$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{Binding Path=$name$, Mode=OneWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ProgressBar Name=\"$safename$\" IsIndeterminate=\"{Binding Path=$name$}\" Height=\"20\" Minimum=\"0\" Maximum=\"1\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$namewithspaces$\" Command=\"{Binding Path=$name$}\" Grid.Row=\"$incint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ListView ItemsSource=\"{Binding Path=$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\"><ListView.ItemTemplate><DataTemplate><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ItemsControl ItemsSource=\"{Binding Path=$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ComboBox Grid.Row=\"$repint$\" Grid.Column=\"1\">$members$</ComboBox>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForXfStackLayout()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool",
                                NameContains = "busy|active",
                                Output = "<ActivityIndicator IsRunning=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Entry Keyboard=\"Default\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<Entry IsPassword=\"True\" Text=\"{Binding $name$}\" Keyboard=\"Default\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBar Placeholder=\"Search\" Text=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel|cell",
                                Output = "<Entry Keyboard=\"Telephone\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "chat|message",
                                Output = "<Entry Keyboard=\"Chat\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<Entry Keyboard=\"Email\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<Entry Keyboard=\"Url\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Image Source=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "uri",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Entry Keyboard=\"Numeric\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "TimeSpan",
                                NameContains = string.Empty,
                                Output = "<TimePicker Time=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset|TimeSpan",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Text=\"$namewithspaces$\" Command=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{Binding $name$}\"><ListView.ItemTemplate><DataTemplate><ViewCell><StackLayout>$subprops$</StackLayout></ViewCell></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<Picker ItemsSource=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<Picker><Picker.Items>$members$</Picker.Items></Picker>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForXfStackLayoutPlusLabels()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool",
                                NameContains = "busy|active",
                                Output = "<Label Text=\"$namewithspaces$\" /><ActivityIndicator IsRunning=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><Entry Keyboard=\"Default\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<Label Text=\"$namewithspaces$\" /><Entry IsPassword=\"True\" Text=\"{Binding $name$}\" Keyboard=\"Default\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<Label Text=\"$namewithspaces$\" /><SearchBar Placeholder=\"Search\" Text=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel|cell",
                                Output = "<Label Text=\"$namewithspaces$\" /><Entry Keyboard=\"Telephone\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "chat|message",
                                Output = "<Label Text=\"$namewithspaces$\" /><Entry Keyboard=\"Chat\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<Label Text=\"$name$\" /><Entry Keyboard=\"Email\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<Label Text=\"$name$\" /><Entry Keyboard=\"Url\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Label Text=\"$namewithspaces$\" /><Image Source=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "uri",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><Entry Keyboard=\"Numeric\" Text=\"{Binding $name$}\" Placeholder=\"$namewithspaces$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "date",
                                Output = "<Label Text=\"$namewithspaces$\" /><DatePicker Date=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "TimeSpan",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><TimePicker Time=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset|TimeSpan",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><Button Text=\"$name$\" Command=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" /><ListView ItemsSource=\"{Binding $name$}\"><ListView.ItemTemplate><DataTemplate><ViewCell><StackLayout>$subprops$</StackLayout></ViewCell></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<Picker Title=\"$namewithspaces$\" ItemsSource=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<Picker Title=\"$namewithspaces$\"><Picker.Items>$members$</Picker.Items></Picker>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForXf2ColGrid()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool",
                                NameContains = "busy|active",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ActivityIndicator IsRunning=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Label Text=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Entry Keyboard=\"Default\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Entry IsPassword=\"True\" Text=\"{Binding $name$}\" Keyboard=\"Default\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><SearchBar Placeholder=\"Search\" Text=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel|cell",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Entry Keyboard=\"Telephone\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "chat|message",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Entry Keyboard=\"Chat\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Entry Keyboard=\"Email\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Entry Keyboard=\"Url\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string|uri",
                                NameContains = "thumbnail|picture|image",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Image Source=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "uri",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Label Text=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Label Text=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Entry Keyboard=\"Numeric\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "date",
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><DatePicker Date=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "TimeSpan",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TimePicker Time=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset|TimeSpan",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Label Text=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Button Text=\"$name$\" Command=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ListView ItemsSource=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\"><ListView.ItemTemplate><DataTemplate><ViewCell><StackLayout>$subprops$</StackLayout></ViewCell></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Picker ItemsSource=\"{Binding $name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"$namewithspaces$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Picker Grid.Row=\"$repint$\" Grid.Column=\"1\" ><Picker.Items>$members$</Picker.Items></Picker>",
                                IfReadOnly = false,
                            },
                        };
        }
    }
}
