// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
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

            // Pick a random profile to show in tab - makes repeated testing more interesting
            this.ProfileConfig.SetDataContextAndHost(ConfiguredSettings.GetDefaultSettings().Profiles.OrderBy(p => Guid.NewGuid()).First(), null);
            this.ProfileConfig.DisableButtonsForEmulator();
        }
    }
}
