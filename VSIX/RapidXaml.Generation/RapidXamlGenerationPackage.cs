// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(RapidXamlGenerationPackage.PackageGuidString)]
    public sealed class RapidXamlGenerationPackage : RapidXamlPackage
    {
        public const string PackageGuidString = "c5301a37-5906-4dd7-a1eb-a21a330be695";

#pragma warning disable CS0628 // New protected member declared in sealed class
        protected new async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
#pragma warning restore CS0628 // New protected member declared in sealed class
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await base.InitializeAsync(cancellationToken, progress);
        }
    }
}
