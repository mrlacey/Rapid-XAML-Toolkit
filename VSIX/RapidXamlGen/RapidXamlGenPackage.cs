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
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    [ProvideAutoLoad(UIContextGuids.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(RapidXamlGenPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration("#110", "#112", "0.0.0.0")] // Info on this package for Help/About
    [ProvideOptionPage(typeof(SettingsConfigPage), "RapidXAML", "Profiles", 106, 107, true)]
    public sealed class RapidXamlGenPackage : AsyncPackage
    {
        public const string PackageGuidString = "e1b8a749-70cc-45f3-b3a4-8f62226d2352";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await SharedRapidXamlPackage.InitializeAsync(cancellationToken, this);

                // TODO: localize this package name
                SharedRapidXamlPackage.Logger?.RecordInfo("Rapid XAML Generation");
                SharedRapidXamlPackage.Logger?.RecordInfo(StringRes.Info_IntializingCommands.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger?.RecordInfo(string.Empty);

                await CopyToClipboardCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await SendToToolboxCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await OpenOptionsCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await RapidXamlDropHandlerProvider.InitializeAsync(this, SharedRapidXamlPackage.Logger);

                // Set the ServiceProvider of CodeParserBase as it's needed to get settings
                CodeParserBase.ServiceProvider = this;

                if (SharedRapidXamlPackage.Logger != null)
                {
                    SharedRapidXamlPackage.Logger.UseExtendedLogging = CodeParserBase.GetSettings().ExtendedOutputEnabled;
                }
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
