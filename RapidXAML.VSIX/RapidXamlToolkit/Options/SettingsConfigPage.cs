// <copyright file="SettingsConfigPage.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Guid("BFE5EED0-5585-4AD3-86C5-808CE50CF190")]
    public class SettingsConfigPage : UIElementDialogPage
    {
        private SettingsControl settingsControl;

        protected override UIElement Child => this.settingsControl ?? (this.settingsControl = new SettingsControl());

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            this.settingsControl.SettingsProvider = this.GetConfiguredSettings();
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            this.settingsControl.SettingsProvider.Save();

            base.OnApply(e);
        }

        private ConfiguredSettings GetConfiguredSettings()
        {
            return new ConfiguredSettings(this.Site);
        }
    }
}
