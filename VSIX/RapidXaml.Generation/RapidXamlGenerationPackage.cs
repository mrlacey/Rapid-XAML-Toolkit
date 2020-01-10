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
    [InstalledProductRegistration("#110", "#112", "0.8.2")] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(RapidXamlGenerationPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(SettingsConfigPage), "Rapid XAML", "Profiles", 106, 107, true)]
    public sealed class RapidXamlGenerationPackage : AsyncPackage
    {
        public const string PackageGuidString = "ad4704fc-2e81-4406-9833-084d6642cf5b";

        public static readonly Guid GenerationCommandSet = new Guid("8c20aab1-50b0-4523-8d9d-24d512fa8154");

        public RapidXamlGenerationPackage()
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

                SharedRapidXamlPackage.Logger.RecordNotice(StringRes.Info_LaunchVersionGeneration.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger.RecordNotice(string.Empty);

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
