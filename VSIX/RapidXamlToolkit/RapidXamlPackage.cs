// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using RapidXaml;
using RapidXaml.EditorExtras;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;

namespace RapidXamlToolkit
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)] // Info on this package for Help/About
    [Guid(PackageGuids.guidRapidXamlToolkitPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(EditorExtrasOptionsGrid), "Rapid XAML", "Editor", 106, 107, true)]
    [ProvideProfile(typeof(EditorExtrasOptionsGrid), "Rapid XAML", "Editor", 106, 107, true)]
    public sealed class RapidXamlPackage : AsyncPackage
    {
        public static EditorExtrasOptionsGrid EditorOptions { get; internal set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await SharedRapidXamlPackage.InitializeAsync(cancellationToken, this);

                SharedRapidXamlPackage.Logger?.RecordNotice(StringRes.Info_LaunchVersionRxt.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger?.RecordNotice(string.Empty);

                RapidXamlPackage.EditorOptions = (EditorExtrasOptionsGrid)this.GetDialogPage(typeof(EditorExtrasOptionsGrid));
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
