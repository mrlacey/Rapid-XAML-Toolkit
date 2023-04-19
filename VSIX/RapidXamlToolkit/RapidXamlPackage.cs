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
using Microsoft.VisualStudio.Shell.Interop;
using RapidXaml;
using RapidXaml.EditorExtras;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Configuration;
using RapidXamlToolkit.DragDrop;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit
{
#if !DEBUG
#error Replace the local reference to RapidXaml.CustomAnalysis with one to the NuGet package before releasing to the marketplace.
#endif

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

        public const string TelemetryGuid = "c735dfc3-c416-4501-bc33-558e2aaad8c5";

        public static ILogger Logger { get; set; }

        public static AsyncPackage Instance { get; private set; }

        public static bool IsLoaded { get; private set; }

        public static AnalysisOptionsGrid AnalysisOptions { get; internal set; }

        public static EditorExtrasOptionsGrid EditorOptions { get; internal set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await this.InitializeLoggerAsync(cancellationToken);

                Logger?.RecordNotice(StringRes.Info_LaunchVersionRxt.WithParams(CoreDetails.GetVersion()));
                Logger?.RecordNotice(string.Empty);

                await FeedbackCommand.InitializeAsync(this, Logger);
                await MoveAllHardCodedStringsToResourceFileCommand.InitializeAsync(this, Logger);
                await AnalyzeCurrentDocumentCommand.InitializeAsync(this, Logger);
                await OpenAnalysisOptionsCommand.InitializeAsync(this, Logger);

                await this.SetUpRunningDocumentTableEventsAsync(cancellationToken);
                RapidXamlDocumentCache.Initialize(this, Logger);

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

                await CopyToClipboardCommand.InitializeAsync(this, Logger);
                await SendToToolboxCommand.InitializeAsync(this, Logger);
                await OpenOptionsCommand.InitializeAsync(this, Logger);
                await RapidXamlDropHandlerProvider.InitializeAsync(this, Logger);

                // Set the ServiceProvider of CodeParserBase as it's needed to get settings
                CodeParserBase.ServiceProvider = this;

                if (Logger != null)
                {
                    Logger.UseExtendedLogging = CodeParserBase.GetSettings().ExtendedOutputEnabled;
                }
            }
            catch (Exception exc)
            {
                Logger?.RecordException(exc);
            }
        }

        private async Task InitializeLoggerAsync(CancellationToken cancellationToken)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await RapidXamlPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                if (Logger == null)
                {
                    var rxtLogger = new RxtLogger();

                    var config = new RxtSettings();

                    var telemLogger = TelemetryAccessor.Create(rxtLogger, config.AppInsightsConnectionString);

                    Logger = new RxtLoggerWithTelemtry(rxtLogger, telemLogger);

                    var activityLog = await RapidXamlPackage.Instance.GetServiceAsync<SVsActivityLog, IVsActivityLog>();
                    rxtLogger.VsActivityLog = activityLog;
                }

                // The RxtOutputPane is used by all extensions
                // so using that as a way to tell if any extensions have initialized.
                // Only want the default info loading once.
                if (!RxtOutputPane.IsInitialized())
                {
                    Logger?.RecordNotice(StringRes.Info_ProblemsInstructionsAndLink);
                    Logger?.RecordNotice(string.Empty);
                }
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
