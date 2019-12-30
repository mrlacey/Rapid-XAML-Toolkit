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
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "0.8.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(RapidXamlAnalysisPackage.PackageGuidString)]
    public sealed class RapidXamlAnalysisPackage : AsyncPackage
    {
        public const string PackageGuidString = "fd0b0440-83be-4d1b-a449-9ca75d53007c";

        public static readonly Guid AnalysisCommandSet = new Guid("f1a4455d-b523-4b08-8ff7-2a964177fcf6");

        public RapidXamlAnalysisPackage()
        {
        }

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

                // TODO: localize this package name
                SharedRapidXamlPackage.Logger?.RecordNotice("Rapid XAML Analysis");
                SharedRapidXamlPackage.Logger?.RecordNotice(StringRes.Info_IntializingCommands.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger?.RecordNotice(string.Empty);

                await FeedbackCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await MoveAllHardCodedStringsToResourceFileCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);

                await this.SetUpRunningDocumentTableEventsAsync(cancellationToken);
                RapidXamlDocumentCache.Initialize(this, SharedRapidXamlPackage.Logger);

                Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterCloseSolution += this.HandleCloseSolution;
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
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
