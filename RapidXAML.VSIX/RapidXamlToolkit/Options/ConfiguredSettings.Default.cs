// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RapidXamlToolkit.Options
{
    public partial class ConfiguredSettings
    {
        public static Settings GetDefaultSettings()
        {
            return new Settings
            {
                ExtendedOutputEnabled = true,  // TODO: ISSUE#45 - Change this to false for 1.0 but want it on by default for testers
                ActiveProfileName = string.Empty,  // No profile should be selected by default
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        Name = "UWP C# MVVM Basic StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanel(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using Windows.UI.Xaml.Controls;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewmodelclass$ ViewModel { get; } = new $viewmodelclass$();

        public $viewclass$()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = "public $viewmodelns$.$viewmodelclass$ ViewModel { get; } = new $viewmodelns$.$viewmodelclass$();",
                            CodeBehindConstructorContent = "DataContext = ViewModel;",
                            DefaultCodeBehindConstructor = @"public $viewclass$()
{
    InitializeComponent();
}",
                        },
                    },

                    new Profile
                    {
                        Name = "UWP C# MVVM Basic 2ColGrid",
                        ClassGrouping = "GRID-PLUS-ROWDEFS-2COLS",
                        FallbackOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$repint$\" Grid.Column=\"1\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsFor2ColGrid(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using Windows.UI.Xaml.Controls;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewmodelclass$ ViewModel { get; } = new $viewmodelclass$();

        public $viewclass$()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = "public $viewmodelns$.$viewmodelclass$ ViewModel { get; } = new $viewmodelns$.$viewmodelclass$();",
                            CodeBehindConstructorContent = "DataContext = ViewModel;",
                            DefaultCodeBehindConstructor = @"public $viewclass$()
{
    InitializeComponent();
}",
                        },
                    },

                    new Profile
                    {
                        Name = "UWP C# MVVM Light StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanel(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    DataContext=""{Binding $viewmodelclass$, Source={StaticResource Locator}}""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using Windows.UI.Xaml.Controls;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        private $viewmodelclass$ ViewModel
        {
            get { return DataContext as $viewmodelclass$; }
        }

        public $viewclass$()
        {
            InitializeComponent();
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = "DataContext=\"{Binding $viewmodelclass$, Source={StaticResource Locator}}\"",
                            CodeBehindPageContent = @"private $viewmodelclass$ ViewModel
    {
        get { return DataContext as $viewmodelclass$;
    }
}",
                            CodeBehindConstructorContent = string.Empty,
                            DefaultCodeBehindConstructor = @"public $viewclass$()
{
    InitializeComponent();
}",
                        },
                    },

                    new Profile
                    {
                        Name = "UWP C# Caliburn.Micro StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanel(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using Windows.UI.Xaml.Controls;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewclass$()
        {
            InitializeComponent();
        }

        private $viewmodelclass$ ViewModel
        {
            get { return DataContext as $viewmodelclass$; }
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = @"private $viewmodelclass$ ViewModel
    {
        get { return DataContext as $viewmodelclass$;
    }
}",
                            CodeBehindConstructorContent = string.Empty,
                            DefaultCodeBehindConstructor = @"public $viewclass$()
{
    InitializeComponent();
}",
                        },
                    },

                    new Profile
                    {
                        Name = "UWP C# Prism StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanel(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:prismMvvm=""using:Prism.Windows.Mvvm""
    prismMvvm:ViewModelLocator.AutoWireViewModel=""True""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using Windows.UI.Xaml.Controls;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewclass$()
        {
            InitializeComponent();
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = @"xmlns:prismMvvm=""using:Prism.Windows.Mvvm""
    prismMvvm:ViewModelLocator.AutoWireViewModel=""True""",
                            CodeBehindPageContent = "private MainViewModel ViewModel => DataContext as MainViewModel;",
                            CodeBehindConstructorContent = string.Empty,
                            DefaultCodeBehindConstructor = @"public $viewclass$()
{
    InitializeComponent();
}",
                        },
                    },

                    new Profile
                    {
                        Name = "UWP VB MVVM Basic StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanel(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"
Imports $viewmodelns$

Namespace $viewns$
    Public NotInheritable Partial Class $viewclass$
        Inherits Page

        Public Sub New()
            Me.InitializeComponent()
            DataContext = ViewModel
        End Sub

        Property ViewModel as $viewmodelclass$ = New $viewmodelclass$

    End Class
End Namespace
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = "Property ViewModel as $viewmodelns$.$viewmodelclass$ = New $viewmodelns$.$viewmodelclass$",
                            CodeBehindConstructorContent = "DataContext = ViewModel",
                            DefaultCodeBehindConstructor = @"Public Sub New()
    Me.InitializeComponent()
End Sub",
                        },
                    },

                    new Profile
                    {
                        Name = "UWP VB MVVM Basic 2ColGrid",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsFor2ColGrid(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"
Imports $viewmodelns$

Namespace $viewns$
    Public NotInheritable Partial Class $viewclass$
        Inherits Page

        Public Sub New()
            Me.InitializeComponent()
            DataContext = ViewModel
        End Sub

        Property ViewModel as $viewmodelclass$ = New $viewmodelclass$

    End Class
End Namespace
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = "Property ViewModel as $viewmodelns$.$viewmodelclass$ = New $viewmodelns$.$viewmodelclass$",
                            CodeBehindConstructorContent = "DataContext = ViewModel",
                            DefaultCodeBehindConstructor = @"Public Sub New()
    Me.InitializeComponent()
End Sub",
                        },
                    },

                    new Profile
                    {
                        Name = "UWP VB MVVM Light StackPanel",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$, Mode=OneWay}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = MappingsForStackPanel(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    DataContext=""{Binding $viewmodelclass$, Source={StaticResource Locator}}""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"Imports $viewmodelns$

Namespace $viewns$
    Public NotInheritable Partial Class $viewclass$
        Inherits Page

        Private ReadOnly Property ViewModel As $viewmodelclass$
            Get
                Return TryCast(DataContext, $viewmodelclass$)
            End Get
        End Property

    End Class
End Namespace
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = "DataContext=\"{Binding $viewmodelclass$, Source={StaticResource Locator}}\"",
                            CodeBehindPageContent = @"Private ReadOnly Property ViewModel As $viewmodelclass$
    Get
        Return TryCast(DataContext, $viewmodelclass$)
    End Get
End Property",
                            CodeBehindConstructorContent = string.Empty,
                            DefaultCodeBehindConstructor = @"Public sub New
    InitializeComponent()
End Sub",
                        },
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms C# StackLayout",
                        ClassGrouping = "StackLayout",
                        FallbackOutput = "<Label Text=\"{Binding $name$}\" />",
                        SubPropertyOutput = "<Label Text=\"{Binding $name$}\" />",
                        EnumMemberOutput = " <x:String>$element$</x:String>",
                        Mappings = MappingsForStackLayout(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
             xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
             x:Class=""$viewns$.$viewclass$""
             Title=""{Binding Title}""
             x:Name=""BrowseItemsPage"">
    <ContentPage.Content>
        $genxaml$
    </ContentPage.Content>
</ContentPage>
",
                            CodePlaceholder = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using $viewmodelns$;

namespace CsXf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class $viewclass$ : ContentPage
    {
        $viewmodelclass$ viewModel;

        public $viewclass$()
        {
            InitializeComponent();

            BindingContext = viewModel = new $viewmodelclass$();
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = "$viewmodelclass$ viewModel;",
                            CodeBehindConstructorContent = "BindingContext = viewModel = new $viewmodelclass$();",
                            DefaultCodeBehindConstructor = @"public $viewclass$()
{
    InitializeComponent();
}",
                        },
                    },

#if DEBUG
                    // This is just a dummy entry to separate useful (above) vs demo/test profiles (below)
                    new Profile
                    {
                        Name = "---------------------",
                        ClassGrouping = string.Empty,
                        FallbackOutput = string.Empty,
                        SubPropertyOutput = string.Empty,
                        EnumMemberOutput = string.Empty,
                        Mappings = new ObservableCollection<Mapping>(),
                        ViewGeneration = new ViewGenerationSettings
                        {
                            AllInSameProject = true,
                            CodePlaceholder = string.Empty,
                            ViewModelDirectoryName = string.Empty,
                            ViewModelFileSuffix = string.Empty,
                            ViewModelProjectSuffix = string.Empty,
                            XamlFileDirectoryName = string.Empty,
                            XamlFileSuffix = string.Empty,
                            XamlPlaceholder = string.Empty,
                            XamlProjectSuffix = string.Empty,
                        },
                        Datacontext = new DatacontextSettings
                        {
                            CodeBehindConstructorContent = string.Empty,
                            CodeBehindPageContent = string.Empty,
                            DefaultCodeBehindConstructor = string.Empty,
                            XamlPageAttribute = string.Empty,
                        },
                    },

                    new Profile
                    {
                        Name = "Demo-Basic",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
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
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
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
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{x:Bind ViewModel.$name$}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "$members$",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewmodelclass$ ViewModel { get; set; }

        public $viewclass$()
        {
            this.InitializeComponent();
            this.ViewModel = new $viewmodelclass$();
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = "private $viewmodelclass$ ViewModel { get { return DataContext as $viewmodelclass$; } }",
                            CodeBehindConstructorContent = "this.DataContext = this.ViewModel;",
                            DefaultCodeBehindConstructor = @"public $viewclass$()
{
    this.InitializeComponent();
}",
                        },
                    },

                    new Profile
                    {
                        Name = "Demo-Basic VB",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$}\" />",
                        EnumMemberOutput = "<RadioButton Content=\"$element$\" GroupName=\"$enumname$\" />",
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
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
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
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
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{x:Bind ViewModel.$name$}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "enum",
                                NameContains = string.Empty,
                                Output = "$members$",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"Imports $viewmodelns$

Public NotInheritable Class $viewclass$
    Inherits Page

End Class
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = @"Private ReadOnly Property ViewModel As $viewmodelclass$
    Get
        Return TryCast(DataContext, $viewmodelclass$)
    End Get
End Property",
                            CodeBehindConstructorContent = "DataContext = ViewModel",
                            DefaultCodeBehindConstructor = @"    Sub New()
        InitializeComponent()
    End Sub",
                        },
                    },

                    new Profile
                    {
                        Name = "Demo-Light",
                        ClassGrouping = "Grid-plus-RowDefs",
                        FallbackOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBlock Text=\"{x:Bind $name$}\" Grid.Row=\"$incint$\" />",
                        EnumMemberOutput = string.Empty,
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<ToggleSwitch Header=\"$name$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"Url\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$name$\" Command=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<ListView ItemsSource=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\">\n<ListView.ItemTemplate>\n<DataTemplate x:DataType=\"$type$\">\n<GRID-PLUS-ROWDEFS>$subprops$</GRID-PLUS-ROWDEFS>\n</DataTemplate>\n</ListView.ItemTemplate>\n</ListView>",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    DataContext=""{Binding $viewmodelclass$, Source={StaticResource Locator}}""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        private $viewmodelclass$ ViewModel
        {
            get { return DataContext as $viewmodelclass$; }
        }

        public $viewclass$()
        {
            this.InitializeComponent();
        }
    }
}
",
                            XamlFileSuffix = "View",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = string.Empty,
                            ViewModelProjectSuffix = ".ViewModels",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = "DataContext=\"{Binding $viewmodelclass$, Source={StaticResource Locator}}\"",
                            CodeBehindPageContent = "private $viewmodelclass$ ViewModel { get { return DataContext as $viewmodelclass$; } }",
                            CodeBehindConstructorContent = string.Empty,
                            DefaultCodeBehindConstructor = string.Empty,
                        },
                    },

                    new Profile
                    {
                        Name = "Demo-Simple",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$}\" />",
                        EnumMemberOutput = string.Empty,
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
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
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
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
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{x:Bind ViewModel.$name$}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewmodelclass$ ViewModel { get; set; }

        public $viewclass$()
        {
            this.InitializeComponent();
            this.ViewModel = new $viewmodelclass$();
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Pages",
                            ViewModelDirectoryName = string.Empty,

                            AllInSameProject = false,

                            XamlProjectSuffix = string.Empty,
                            ViewModelProjectSuffix = ".ViewModels",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = "private $viewmodelclass$ ViewModel { get { return DataContext as $viewmodelclass$; } }",
                            CodeBehindConstructorContent = "this.DataContext = this.ViewModel;",
                            DefaultCodeBehindConstructor = string.Empty,
                        },
                    },

                    new Profile
                    {
                        Name = "UWP",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{x:Bind $name$}\" />",
                        EnumMemberOutput = string.Empty,
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"{x:Bind ViewModel.$name$}\" />",
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
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" />",
                                IfReadOnly = true,
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
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />",
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
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{x:Bind ViewModel.$name$}\"><ListView.ItemTemplate><DataTemplate x:DataType=\"$type$\"><StackPanel>$subprops$</StackPanel></DataTemplate></ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewmodelclass$ ViewModel { get; set; }

        public $viewclass$()
        {
            this.InitializeComponent();
            this.ViewModel = new $viewmodelclass$();
        }
    }
}
",
                            XamlFileSuffix = "Page",
                            ViewModelFileSuffix = "ViewModel",

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = "ViewModels",

                            AllInSameProject = true,

                            XamlProjectSuffix = "n/a",
                            ViewModelProjectSuffix = "n/a",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = "DataContext=\"{Binding $viewmodelclass$, Source={StaticResource Locator}}\"",
                            CodeBehindPageContent = "private $viewmodelclass$ ViewModel { get { return DataContext as $viewmodelclass$; } }", // Caliburn Micro style
                            CodeBehindConstructorContent = "this.DataContext = this.ViewModel;", // MVVMBasic style
                            DefaultCodeBehindConstructor = string.Empty,
                        },
                    },

                    new Profile
                    {
                        Name = "UWP (with labels)",
                        ClassGrouping = "Grid-plus-RowDefs",
                        FallbackOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBlock Text=\"{x:Bind $name$}\" Grid.Row=\"$incint$\" />",
                        EnumMemberOutput = string.Empty,
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<ProgressRing IsActive=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<ToggleSwitch Header=\"$name$\" IsOn=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<SearchBox PlaceholderText=\"Search\" QueryText=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<PasswordBox Password=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"TelephoneNumber\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"EmailNameOrAddress\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "firstname|lastname|familyname|surname|givenname",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"PersonalFullName\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox InputScope=\"Url\" Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBlock Text=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<TextBox Text=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$name$\" Command=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset",
                                NameContains = "date",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "Uri",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<HyperlinkButton NavigateUri=\"{x:Bind ViewModel.$name$}\" Content=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<ItemsControl ItemsSource=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />\n<ListView ItemsSource=\"{x:Bind ViewModel.$name$}\" Grid.Row=\"$incint$\">\n<ListView.ItemTemplate>\n<DataTemplate x:DataType=\"$type$\">\n<GRID-PLUS-ROWDEFS>$subprops$</GRID-PLUS-ROWDEFS>\n</DataTemplate>\n</ListView.ItemTemplate>\n</ListView>",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            XamlPlaceholder = @"<Page
    x:Class=""$viewns$.$viewclass$""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:$viewns$""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        $genxaml$
    </Grid>
</Page>
",
                            CodePlaceholder = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewmodelclass$ ViewModel { get; set; }

        public $viewclass$()
        {
            this.InitializeComponent();
            this.ViewModel = new $viewmodelclass$();
        }
    }
}
",
                            XamlFileSuffix = "View",
                            ViewModelFileSuffix = string.Empty,

                            XamlFileDirectoryName = "Views",
                            ViewModelDirectoryName = string.Empty,

                            AllInSameProject = false,

                            XamlProjectSuffix = string.Empty,
                            ViewModelProjectSuffix = ".ViewModels",
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = "DataContext=\"{Binding $viewmodelclass$, Source={StaticResource Locator}}\"",
                            CodeBehindPageContent = "private $viewmodelclass$ ViewModel { get { return DataContext as $viewmodelclass$; } }", // Caliburn Micro style
                            CodeBehindConstructorContent = "this.DataContext = this.ViewModel;", // MVVMBasic style
                            DefaultCodeBehindConstructor = string.Empty,
                        },
                    },

                    new Profile
                    {
                        Name = "WPF",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"{Binding Path=$name$}\" />",
                        SubPropertyOutput = "<TextBlock Text=\"{Binding Path=$name$}\" />",
                        EnumMemberOutput = string.Empty,
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ProgressBar IsIndeterminate=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = string.Empty,
                                Output = "<CheckBox IsChecked=\"{Binding $name$, Mode=TwoWay}\"><TextBlock Text=\"$name$\" /></CheckBox>",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{Binding $name$, Mode=TwoWay}\" />",
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
                                Output = "<TextBlock Text=\"{Binding $name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBox Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ICommand|Command|RelayCommand",
                                NameContains = string.Empty,
                                Output = "<Button Content=\"$name$\" Command=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTime",
                                NameContains = "date",
                                Output = "<DatePicker DisplayDate=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{Binding $name$}\">\n<ListView.ItemTemplate>\n<DataTemplate x:DataType=\"$type$\">\n<StackPanel>$subprops$</StackPanel>\n</DataTemplate>\n</ListView.ItemTemplate>\n</ListView>",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            AllInSameProject = true,
                            CodePlaceholder = string.Empty,
                            ViewModelDirectoryName = string.Empty,
                            ViewModelFileSuffix = string.Empty,
                            ViewModelProjectSuffix = string.Empty,
                            XamlFileDirectoryName = string.Empty,
                            XamlFileSuffix = string.Empty,
                            XamlPlaceholder = string.Empty,
                            XamlProjectSuffix = string.Empty,
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = string.Empty,
                            CodeBehindConstructorContent = string.Empty,
                            DefaultCodeBehindConstructor = string.Empty,
                        },
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms",
                        ClassGrouping = "StackLayout",
                        FallbackOutput = "<Label Text=\"{Binding $name$}\" />",
                        SubPropertyOutput = "<Label Text=\"{Binding $name$}\" />",
                        EnumMemberOutput = string.Empty,
                        Mappings = new ObservableCollection<Mapping>
                        {
                            new Mapping
                            {
                                Type = "bool|Boolean",
                                NameContains = "busy|active",
                                Output = "<ActivityIndicator IsRunning=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "password|pwd",
                                Output = "<Entry IsPassword=\"True\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<Entry Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = string.Empty,
                                Output = "<Label Text=\"{Binding $name$}\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "search",
                                Output = "<SearchBar Placeholder=\"Search\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "phone|tel",
                                Output = "<Entry Keyboard=\"Telephone\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "chat|message",
                                Output = "<Entry Keyboard=\"Chat\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "email",
                                Output = "<Entry Keyboard=\"Email\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "string",
                                NameContains = "uri|url",
                                Output = "<Entry Keyboard=\"Url\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "int|Integer|long|double|float",
                                NameContains = string.Empty,
                                Output = "<Entry Keyboard=\"Numeric\" Text=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset|DateTime",
                                NameContains = "date",
                                Output = "<DatePicker Date=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "DateTimeOffset|DateTime",
                                NameContains = "time",
                                Output = "<TimePicker Time=\"{Binding $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
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
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<Picker ItemsSource=\"{Binding $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "ObservableCollection<T>|List<T>",
                                NameContains = string.Empty,
                                Output = "<ListView ItemsSource=\"{Binding $name$}\">\n<ListView.ItemTemplate>\n<DataTemplate x:DataType=\"$type$\">\n<StackLayout>$subprops$</StackLayout>\n</DataTemplate>\n</ListView.ItemTemplate></ListView>",
                                IfReadOnly = false,
                            },
                        },
                        ViewGeneration = new ViewGenerationSettings
                        {
                            AllInSameProject = true,
                            CodePlaceholder = string.Empty,
                            ViewModelDirectoryName = string.Empty,
                            ViewModelFileSuffix = string.Empty,
                            ViewModelProjectSuffix = string.Empty,
                            XamlFileDirectoryName = string.Empty,
                            XamlFileSuffix = string.Empty,
                            XamlPlaceholder = string.Empty,
                            XamlProjectSuffix = string.Empty,
                        },
                        Datacontext = new DatacontextSettings
                        {
                            XamlPageAttribute = string.Empty,
                            CodeBehindPageContent = string.Empty,
                            CodeBehindConstructorContent = string.Empty,
                            DefaultCodeBehindConstructor = string.Empty,
                        },
                    },
#endif
                },
            };
        }

        private static ObservableCollection<Mapping> MappingsForStackPanel()
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
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" Grid.Column=\"0\" /><DatePicker Date=\"{x:Bind ViewModel.$name$, Mode=TwoWay}\" />Grid.Row=\"$repint$\" Grid.Column=\"1\" ",
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
