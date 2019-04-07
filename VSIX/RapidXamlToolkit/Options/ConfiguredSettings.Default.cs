// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using RapidXamlToolkit.Configuration;

namespace RapidXamlToolkit.Options
{
    public partial class ConfiguredSettings
    {
        public static Settings GetDefaultSettings()
        {
            var config = new RxtSettings();

            return new Settings
            {
                ExtendedOutputEnabled = config.ExtendedOutputEnabledByDefault,  // Disable for release builds but want it on by default for testers
                ActiveProfileName = string.Empty,  // No profile should be selected by default
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        Name = "UWP C# MVVM Basic StackPanel (no headers)",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithoutHeaders(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP C# MVVM Basic StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP C# MVVM Basic 2ColGrid",
                        ClassGrouping = "GRID-PLUS-ROWDEFS-2COLS",
                        FallbackOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsFor2ColGrid(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP C# MVVM Basic RelativePanel",
                        ClassGrouping = "RelativePanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForRelativePanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP C# MVVM Light StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP C# Caliburn.Micro StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP C# Prism StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP VB MVVM Basic StackPanel (no headers)",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithoutHeaders(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP VB MVVM Basic StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP VB MVVM Basic 2ColGrid",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsFor2ColGrid(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "UWP VB MVVM Light StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanelWithHeader(),
                        AttemptAutomaticDocumentFormatting = true,
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms StackLayout (XAML + C#)",
                        ClassGrouping = "StackLayout",
                        FallbackOutput = "<Label Text=\"{Binding $name$}\" />",
                        SubPropertyOutput = "<Label Text=\"{Binding $name$}\" />",
                        EnumMemberOutput = "<x:String>$elementwithspaces$</x:String>",
                        Mappings = MappingsForStackLayout(),
                        AttemptAutomaticDocumentFormatting = true,
                    },
                },
            };
        }

        private static ObservableCollection<Mapping> MappingsForStackPanelWithoutHeaders()
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
                                NameContains = "phone|tel",
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
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Output = "<ToggleSwitch Header=\"$name$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Output = "<Button Content=\"$name$\" Command=\"{x:Bind ViewModel.$name$}\" />",
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

        private static ObservableCollection<Mapping> MappingsForStackPanelWithHeader()
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
                                NameContains = "phone|tel",
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
                                Output = "<Slider Header=\"$namewithspaces$\" Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Output = "<Button Content=\"$name$\" Command=\"{x:Bind ViewModel.$name$}\" />",
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

        private static ObservableCollection<Mapping> MappingsForRelativePanelWithHeader()
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
                                NameContains = "phone|tel",
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
                                Output = "<Button Content=\"$name$\" Command=\"{x:Bind ViewModel.$name$}\" x:Name=\"$xname$\" RelativePanel.Below=\"$repxname$\" />",
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

        private static ObservableCollection<Mapping> MappingsFor2ColGrid()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBox InputScope=\"Url\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><AutoSuggestBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ToggleSwitch Header=\"$name$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><Button Content=\"$name$\" Command=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
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
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" ></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><StackPanel Grid.Row=\"$repint$\" Grid.Column=\"1\" >$members$</StackPanel>",
                                IfReadOnly = false,
                            },
                        };
        }

        private static ObservableCollection<Mapping> MappingsForStackLayout()
        {
            return new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<Entry Keyboard=\"Default\" Text=\"{Binding $name$}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<Entry Keyboard=\"Telephone\" Text=\"{Binding $name$, Mode=TwoWay}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<Entry Keyboard=\"Email\" Text=\"{Binding $name$, Mode=TwoWay}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<Entry Keyboard=\"Url\" Text=\"{Binding $name$, Mode=TwoWay}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "chat|message",
                                Output = "<Entry Keyboard=\"Chat\" Text=\"{Binding $name$, Mode=TwoWay}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBar PlaceholderText=\"Search\" QueryText=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<Entry IsPassword=\"True\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool",
                                NameContains = "busy|active",
                                Output = "<ActivityIndicator IsRunning=\"{Binding $name$}\" />",
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
                                Output = "<Entry Keyboard=\"Numeric\" Text=\"{Binding $name$, Mode=TwoWay}\" Placeholder=\"$name$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = "time",
                                Output = "<TimePicker Time=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime|DateTimeOffset",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Text=\"$name$\" Command=\"{Binding $name$}\" />",
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
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "<Picker Title=\"$name$\"><Picker.Items>$members$</Picker.Items></Picker>",
                                IfReadOnly = false,
                            },
                        };
        }
    }
}
