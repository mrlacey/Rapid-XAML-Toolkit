// <copyright file="ViewGenerationSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RapidXamlToolkit
{
    public class ViewGenerationSettings
    {
        // This is just until this is configurable (ISSUE#21)
        public ViewGenerationSettings()
        {
            this.XamlPlaceholder = @"<Page
    x:Class=""{PROJECT}.Views.{CLASS}Page""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:{PROJECT}.Views""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Grid Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
        {GENXAML}
    </Grid>
</Page>
";

            this.CodePlaceholder = @"using System;
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
using {PROJECT}.ViewModels;

namespace {PROJECT}.Views
{
    public sealed partial class {CLASS}Page : Page
    {
        public {CLASS} ViewModel { get; set; }

        public {CLASS}Page()
        {
            this.InitializeComponent();
            this.ViewModel = new {CLASS}();
        }
    }
}
";
        }

        public string XamlPlaceholder { get; internal set; }

        public string CodePlaceholder { get; internal set; }
    }
}
