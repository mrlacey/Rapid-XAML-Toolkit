// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using RapidXaml;
using RapidXaml.EditorExtras;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.DragDrop;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using RapidXamlToolkit.XamlAnalysis;
using static Microsoft.VisualStudio.VSConstants;

namespace RapidXamlToolkit
{
    [ProvideAutoLoad(ActivationContextGuid, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)] // Info on this package for Help/About
    [Guid(PackageGuids.guidRapidXamlToolkitPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideUIContextRule(
        ActivationContextGuid,
        name: "Load Rapid XAML Package",
        expression: "HasXamlFiles",
        termNames: new[] { "HasXamlFiles" },
        termValues: new[] { "HierSingleSelectionName:.xaml$" })]
    [ProvideOptionPage(typeof(EditorExtrasOptionsGrid), "Rapid XAML", "Editor", 106, 107, true)]
    [ProvideProfile(typeof(EditorExtrasOptionsGrid), "Rapid XAML", "Editor", 106, 107, true)]
    [ProvideOptionPage(typeof(AnalysisOptionsGrid), "Rapid XAML", "Analysis", 106, 104, true)]
    [ProvideProfile(typeof(AnalysisOptionsGrid), "Rapid XAML", "Analysis", 106, 104, true)]
    [ProvideOptionPage(typeof(SettingsConfigPage), "Rapid XAML", "Generation Profiles", 106, 105, true)]
    [ProvideProfile(typeof(SettingsConfigPage), "Rapid XAML", "Generation Profiles", 106, 105, true)]
    public sealed class RapidXamlPackage : AsyncPackage
    {

        public const string ActivationContextGuid = "47A8ECBA-E247-4FE7-80EE-CDE6CEAC03A5";

        public static bool IsLoaded { get; private set; }

        public static AnalysisOptionsGrid AnalysisOptions { get; internal set; }

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
                RapidXamlPackage.IsLoaded = true;

                RapidXamlPackage.AnalysisOptions = (AnalysisOptionsGrid)this.GetDialogPage(typeof(AnalysisOptionsGrid));

                RapidXamlPackage.EditorOptions = (EditorExtrasOptionsGrid)this.GetDialogPage(typeof(EditorExtrasOptionsGrid));

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
