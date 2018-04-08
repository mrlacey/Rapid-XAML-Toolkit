// <copyright file="ConfiguredSettings.Default.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace RapidXamlToolkit
{
    public partial class ConfiguredSettings
    {
        public static Settings GetDefaultSettings()
        {
            return new Settings
            {
                ActiveProfileName = "UWP",
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        Name = "UWP",
                        ClassGrouping = "StackPanel",
                        DefaultOutput = "<TextBlock Text=\"{x:Bind ViewModel.{NAME}}\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBox Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.{NAME}}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"{NAME}\" Value=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.{NAME}}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"{NAME}\" Command=\"{x:Bind ViewModel.{NAME}}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<ToggleSwitch Header=\"{NAME}\" IsOn=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.{NAME}}\" Content=\"{x:Bind ViewModel.{NAME}}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.{NAME}}\" Content=\"{x:Bind ViewModel.{NAME}}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<TextBox InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "postcode|zip",
                                Output = "<TextBox InputScope=\"zipcodeinputscope\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBox InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBox InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBox InputScope=\"Url\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{x:Bind ViewModel.{NAME}}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"{TYPE}\"><StackPanel>{SUBPROPERTIES}</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                        },
                    },

                    new Profile
                    {
                        Name = "UWP (with labels)",
                        ClassGrouping = "Grid-plus-RowDefs",
                        DefaultOutput = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBlock Text=\"{x:Bind ViewModel.{NAME}}\" Grid.Row=\"{X}\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<ProgressRing IsActive=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<ToggleSwitch Header=\"{NAME}\" IsOn=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"{NAME}\" Value=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<PasswordBox Password=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.{NAME}}\" Content=\"{x:Bind ViewModel.{NAME}}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBox InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "postcode|zip",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBox InputScope=\"zipcodeinputscope\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBox InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBox InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBox InputScope=\"Url\" Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBlock Text=\"{x:Bind ViewModel.{NAME}}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<TextBox Text=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"{NAME}\" Command=\"{x:Bind ViewModel.{NAME}}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<DatePicker Date=\"{x:Bind ViewModel.{NAME}, Mode=TwoWay}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.{NAME}}\" Content=\"{x:Bind ViewModel.{NAME}}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<ItemsControl ItemsSource=\"{x:Bind ViewModel.{NAME}}\" Grid.Row=\"{X}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{NAME}\" Grid.Row=\"{X}\" />\n<ListView ItemsSource=\"{x:Bind ViewModel.{NAME}}\" Grid.Row=\"{X}\">\n<ListView.ItemTemplate>\n<DataTemplate x:DataType=\"{TYPE}\">\n<GRID-PLUS-ROWDEFS>{SUBPROPERTIES}</GRID-PLUS-ROWDEFS>\n</DataTemplate>\n</ListView.ItemTemplate>\n</ListView>",
                                IfReadOnly = false,
                            },
                        },
                    },

                    new Profile
                    {
                        Name = "WPF",
                        ClassGrouping = "StackPanel",
                        DefaultOutput = "<TextBlock Text=\"{Binding Path={NAME}}\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressBar IsIndeterminate=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<CheckBox IsChecked=\"{Binding {NAME}, Mode=TwoWay}\"><TextBlock Text=\"{NAME}\" /></CheckBox>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"{NAME}\" Value=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{Binding {NAME}}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBox Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"{NAME}\" Command=\"{Binding {NAME}}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime",
                                NameContains = "date",
                                Output = "<DatePicker DisplayDate=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{Binding {NAME}}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{Binding {NAME}}\">\n<ListView.ItemTemplate>\n<DataTemplate x:DataType=\"{TYPE}\">\n<StackPanel>{SUBPROPERTIES}</StackPanel>\n</DataTemplate>\n</ListView.ItemTemplate>\n</ListView>",
                                IfReadOnly = false,
                            },
                        },
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms",
                        ClassGrouping = "StackLayout",
                        DefaultOutput = "<Label Text=\"{Binding {NAME}}\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ActivityIndicator IsRunning=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "password|pwd",
                                Output = "<Entry IsPassword=\"True\" Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<Entry Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding {NAME}}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBar Placeholder=\"Search\" Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<Entry Keyboard=\"Telephone\" Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "chat|message",
                                Output = "<Entry Keyboard=\"Chat\" Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<Entry Keyboard=\"Email\" Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<Entry Keyboard=\"Url\" Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Entry Keyboard=\"Numeric\" Text=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset|DateTime",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset|DateTime",
                                NameContains = "time",
                                Output = "<TimePicker Time=\"{Binding {NAME}, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Text=\"{NAME}\" Command=\"{Binding {NAME}}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<Picker ItemsSource=\"{Binding {NAME}}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{Binding {NAME}}\">\n<ListView.ItemTemplate>\n<DataTemplate x:DataType=\"{TYPE}\">\n<StackLayout>{SUBPROPERTIES}</StackLayout>\n</DataTemplate>\n</ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                        },
                    },
                },
            };
        }
    }
}
