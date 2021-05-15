// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using RapidXamlToolkit.XamlAnalysis;
using static Microsoft.VisualStudio.VSConstants;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    [ProvideAutoLoad(UICONTEXT.CSharpProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UICONTEXT.VBProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "0.12.0")] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(RapidXamlAnalysisPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(AnalysisOptionsGrid), "Rapid XAML", "Analysis", 106, 107, true)]
    [ProvideProfile(typeof(AnalysisOptionsGrid), "Rapid XAML", "Analysis", 106, 107, true)]
    public sealed class RapidXamlAnalysisPackage : AsyncPackage
    {
        public const string PackageGuidString = "fd0b0440-83be-4d1b-a449-9ca75d53007c";

        public static readonly Guid AnalysisCommandSet = new Guid("f1a4455d-b523-4b08-8ff7-2a964177fcf6");

        public RapidXamlAnalysisPackage()
        {
        }

        public static bool IsLoaded { get; private set; }

        public static AnalysisOptionsGrid Options { get; internal set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await SharedRapidXamlPackage.InitializeAsync(cancellationToken, this);

                SharedRapidXamlPackage.Logger?.RecordNotice(StringRes.Info_LaunchVersionAnalysis.WithParams(CoreDetails.GetVersion()));
                SharedRapidXamlPackage.Logger?.RecordNotice(string.Empty);

                await FeedbackCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await MoveAllHardCodedStringsToResourceFileCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await AnalyzeCurrentDocumentCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);
                await OpenAnalysisOptionsCommand.InitializeAsync(this, SharedRapidXamlPackage.Logger);

                await this.SetUpRunningDocumentTableEventsAsync(cancellationToken);
                RapidXamlDocumentCache.Initialize(this, SharedRapidXamlPackage.Logger);

                Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterCloseSolution += this.HandleCloseSolution;

                // Handle the ability to resolve assemblies when loading custom analyzers.
                // Hat-tip: https://weblog.west-wind.com/posts/2016/dec/12/loading-net-assemblies-out-of-seperate-folders
                AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
                {
                    // Ignore missing resources
                    if (args.Name.Contains(".resources"))
                    {
                        return null;
                    }

                    if (args.RequestingAssembly == null)
                    {
                        return null;
                    }

                    // check for assemblies already loaded
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
                    if (assembly != null)
                    {
                        return assembly;
                    }

                    // Try to load by filename - split out the filename of the full assembly name
                    // and append the base path of the original assembly (ie. look in the same dir)
                    string filename = args.Name.Split(',')[0] + ".dll".ToLower();

                    var asmFile = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.CodeBase), filename);

                    if (asmFile.StartsWith("file:\\"))
                    {
                        asmFile = asmFile.Substring(6);
                    }

                    try
                    {
                        return Assembly.LoadFrom(asmFile);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        return null;
                    }
                };

                // Track this so don't try and load CustomAnalyzers while VS is still starting up.
                RapidXamlAnalysisPackage.IsLoaded = true;

                RapidXamlAnalysisPackage.Options = (AnalysisOptionsGrid)this.GetDialogPage(typeof(AnalysisOptionsGrid));

                var ass = Assembly.GetExecutingAssembly().GetName();

                SharedRapidXamlPackage.Logger?.RecordFeatureUsage(StringRes.Info_PackageLoad.WithParams(ass.Name, ass.Version), quiet: true);
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
