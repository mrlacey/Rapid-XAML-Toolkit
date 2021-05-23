// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using static Microsoft.VisualStudio.VSConstants;
using Task = System.Threading.Tasks.Task;

namespace RapidXaml.EditorExtras
{
    [ProvideAutoLoad(UICONTEXT.CSharpProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UICONTEXT.VBProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "0.12.2")] // Info on this package for Help/About
    [Guid(RapidXamlEditorExtrasPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(EditorExtrasOptionsGrid), "Rapid XAML", "Editor", 106, 107, true)]
    [ProvideProfile(typeof(EditorExtrasOptionsGrid), "Rapid XAML", "Editor", 106, 107, true)]
    public sealed class RapidXamlEditorExtrasPackage : AsyncPackage
    {
        public const string PackageGuidString = "344db717-8a38-400c-b9bc-8f4747ca574b";

        public static EditorExtrasOptionsGrid Options { get; internal set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await SharedRapidXamlPackage.InitializeAsync(cancellationToken, this);

                SharedRapidXamlPackage.Logger?.RecordNotice(StringRes.Info_LaunchVersionEditorExtras.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger?.RecordNotice(string.Empty);

                RapidXamlEditorExtrasPackage.Options = (EditorExtrasOptionsGrid)this.GetDialogPage(typeof(EditorExtrasOptionsGrid));

                var ass = Assembly.GetExecutingAssembly().GetName();

                SharedRapidXamlPackage.Logger?.RecordFeatureUsage(StringRes.Info_PackageLoad.WithParams(ass.Name, ass.Version), quiet: true);
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
