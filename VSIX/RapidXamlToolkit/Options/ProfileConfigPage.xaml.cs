// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.Options
{
    public partial class ProfileConfigPage : ICanClose
    {
        public ProfileConfigPage()
        {
            this.InitializeComponent();
        }

        public void SetDataContext(Profile profile)
        {
            this.ProfileConfig.SetDataContextAndHost(profile, this);
        }
    }
}
