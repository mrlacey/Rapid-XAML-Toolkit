// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using System.Windows;
using RapidXamlToolkit.Options;

namespace OptionsEmulator
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.Settings.DataContext = ConfiguredSettings.GetDefaultSettings();
            this.Settings.DisableButtonsForEmulator();

            this.ProfileConfig.DataContext = ConfiguredSettings.GetDefaultSettings().Profiles.First();
            this.ProfileConfig.DisableButtonsForEmulator();
        }
    }
}
