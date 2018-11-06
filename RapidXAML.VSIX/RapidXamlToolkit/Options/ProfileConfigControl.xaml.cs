// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit.Options
{
    public partial class ProfileConfigControl
    {
        private Profile viewModel;
        private ICanClose host;
        private bool disabled = false;

        public ProfileConfigControl()
        {
            this.InitializeComponent();
        }

        public void SetDataContextAndHost(Profile profile, ICanClose host)
        {
            this.viewModel = profile;
            this.DataContext = this.viewModel;

            this.host = host;
        }

        public void DisableButtonsForEmulator()
        {
            this.disabled = true;
        }

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            this.host?.Close();
        }

        private void AddClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.viewModel.Mappings.Add(Mapping.CreateNew());
                this.viewModel.RefreshMappings();
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (this.DisplayedMappings.SelectedIndex >= 0)
                {
                    var copy = (Mapping)this.viewModel.Mappings[this.DisplayedMappings.SelectedIndex].Clone();

                    this.viewModel.Mappings.Add(copy);
                    this.viewModel.RefreshMappings();
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (this.DisplayedMappings.SelectedIndex >= 0)
                {
                    this.viewModel.Mappings.RemoveAt(this.DisplayedMappings.SelectedIndex);
                    this.viewModel.RefreshMappings();
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }

        private void DetailsClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Microsoft/Rapid-XAML-Toolkit/issues/16");
        }
    }
}
