// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "0.13.0")] // Info on this package for Help/About
    [Guid("ed7fe961-2d10-4598-8040-7423b66b6540")]
    public sealed class RapidXamlPackage : AsyncPackage
    {
    }
}
