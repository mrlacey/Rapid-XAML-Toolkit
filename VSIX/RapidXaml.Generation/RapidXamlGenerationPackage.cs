// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.DragDrop;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    [ProvideAutoLoad(UIContextGuids.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "0.0.0.0")] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(RapidXamlGenerationPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(SettingsConfigPage), "RapidXAML", "Profiles", 106, 107, true)]
    public sealed class RapidXamlGenerationPackage : AsyncPackage
    {
        public const string PackageGuidString = "ad4704fc-2e81-4406-9833-084d6642cf5b";

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
                SharedRapidXamlPackage.Logger.RecordNotice("Rapid XAML Generation");
                SharedRapidXamlPackage.Logger.RecordNotice(StringRes.Info_IntializingCommands.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger.RecordNotice(string.Empty);

                await CopyToClipboardCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await SendToToolboxCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await OpenOptionsCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await RapidXamlDropHandlerProvider.InitializeAsync(this, SharedRapidXamlPackage.Logger);
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
