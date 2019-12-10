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
    [Guid(RapidXamlAnalysisPackage.PackageGuidString)]
    public sealed class RapidXamlAnalysisPackage : RapidXamlPackage
    {
        public const string PackageGuidString = "b20af7e2-1a84-4fb5-ba44-c1bca2eaff8a";

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
