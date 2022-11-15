// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(RapidXamlToolkit.Vsix.Name, RapidXamlToolkit.Vsix.Description, RapidXamlToolkit.Vsix.Version)] // Info on this package for Help/About
    [Guid(PackageGuids.guidRapidXamlToolkitPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class RapidXamlPackage : AsyncPackage
    {
    }
}
