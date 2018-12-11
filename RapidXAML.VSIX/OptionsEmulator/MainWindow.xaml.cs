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

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Pick a random profile to show in tab - makes repeated manual testing more interesting
                this.ProfileConfig.SetDataContextAndHost(ConfiguredSettings.GetDefaultSettings().Profiles.OrderBy(p => Guid.NewGuid()).First(), null);
            }
            else
            {
                // Always use the same in automated testing so can compare visuals
                this.ProfileConfig.SetDataContextAndHost(ConfiguredSettings.GetDefaultSettings().Profiles.First(), null);
            }

            this.ProfileConfig.DisableButtonsForEmulator();
        }
    }
}
