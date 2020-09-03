// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace RapidXaml.Templates
{
    [InstalledProductRegistration("#110", "#112", "0.10.4")] // Info on this package for Help/About
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(RapidXamlTemplatesPackage.PackageGuidString)]
    public sealed class RapidXamlTemplatesPackage : AsyncPackage
    {
        public const string PackageGuidString = "68e98cee-c636-4262-abea-0bdd809739fe";
    }
}
