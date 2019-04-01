// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit.Options
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
            this.settingsControl.SelectFirstProfileInList();
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.settingsControl.SettingsProvider.Save();

            base.OnApply(e);
        }

        private ConfiguredSettings GetConfiguredSettings()
        {
            return new ConfiguredSettings(this.Site);
        }
    }
}
