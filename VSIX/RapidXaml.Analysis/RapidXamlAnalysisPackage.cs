// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using RapidXamlToolkit.XamlAnalysis;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    [ProvideAutoLoad(UIContextGuids.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "0.0.0.0")] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(RapidXamlAnalysisPackage.PackageGuidString)]
    public sealed class RapidXamlAnalysisPackage : AsyncPackage
    {
        public const string PackageGuidString = "b20af7e2-1a84-4fb5-ba44-c1bca2eaff8a";

#pragma warning disable CS0628 // New protected member declared in sealed class
        protected new async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
#pragma warning restore CS0628 // New protected member declared in sealed class
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await this.BaseInitializeAsync(cancellationToken, progress);

                // TODO: localize this package name
                Logger.RecordInfo("Rapid XAML Analysis");
                Logger.RecordInfo(StringRes.Info_IntializingCommands.WithParams(CoreDetails.GetVersion()));
                Logger.RecordInfo(string.Empty);

                await FeedbackCommand.InitializeAsync(this, Logger);
                await MoveAllHardCodedStringsToResourceFileCommand.InitializeAsync(this, Logger);

                await this.SetUpRunningDocumentTableEventsAsync(cancellationToken);
                RapidXamlDocumentCache.Initialize(this, Logger);

                Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterCloseSolution += this.HandleCloseSolution;
            }
            catch (Exception exc)
            {
                Logger?.RecordException(exc);
            }
        }

        private void HandleCloseSolution(object sender, EventArgs e)
        {
            TableDataSource.Instance.CleanAllErrors();
        }

        private async Task SetUpRunningDocumentTableEventsAsync(CancellationToken cancellationToken)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var runningDocumentTable = new RunningDocumentTable(this);

            var plugin = new RapidXamlRunningDocTableEvents(this, runningDocumentTable);

            runningDocumentTable.Advise(plugin);
        }
    }
}
