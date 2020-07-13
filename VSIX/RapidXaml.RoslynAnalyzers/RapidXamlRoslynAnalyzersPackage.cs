// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    [ProvideAutoLoad(UIContextGuids.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "0.10.2")] // Info on this package for Help/About
    [Guid(RapidXamlRoslynAnalyzersPackage.PackageGuidString)]
    public sealed class RapidXamlRoslynAnalyzersPackage : AsyncPackage
    {
        public const string PackageGuidString = "021218fd-3667-42e9-8e27-6775cc561445";

#pragma warning disable CS0628 // New protected member declared in sealed class
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
#pragma warning restore CS0628 // New protected member declared in sealed class
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await SharedRapidXamlPackage.InitializeAsync(cancellationToken, this);

                SharedRapidXamlPackage.Logger?.RecordNotice(StringRes.Info_LaunchVersionRoslynAnalyzers.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger?.RecordNotice(string.Empty);

                var ass = Assembly.GetExecutingAssembly().GetName();

                SharedRapidXamlPackage.Logger.RecordFeatureUsage(StringRes.Info_PackageLoad.WithParams(ass.Name, ass.Version), quiet: true);
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
